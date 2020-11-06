using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BallScripts.Utils;

namespace BallScripts.GameLogics{

    [System.Serializable]
    public struct TeamDescribe
    {
        public int id;
        public Color color;
        public string name;
    }

    public class TeamManager : Singleton<TeamManager>
    {
        public Dictionary<int, Team> teams = new Dictionary<int, Team>();

        [Tooltip("本场地的所有队伍")]
        public List<TeamDescribe> describes = new List<TeamDescribe>();
        /// <summary>
        /// 将Player加入指定队伍
        /// </summary>
        /// <param name="player">加入的Player</param>
        /// <param name="teamID">队伍的id</param>
        public void AddToTeam(Player player,int teamID)
        {
            if (!teams.ContainsKey(teamID))
            {
                //是不在teams中的新队伍
                AddTeamByDescribe(GetTeamDescribe(teamID));
            }
            Team team = teams[teamID];
            team.Add(player.id);
            player.team = team;
            Debug.Log($"Player - {player.id} 现在是 {team.ToString()}");
        }

        public void RemoveFromTeam(Player player)
        {
            Team team = player.team;
            player.team = null;
            team.Remove(player.id);
            if (team.Count == 0)
            {
                teams.Remove(team.id);
            }
        }

        public bool IsSameTeam(Player p1,Player p2)
        {
            if (p1.team == null) 
            {
                Debug.Log($"player - {p1.id} 还没有队伍");
                return false;
            }
            if(p2.team == null)
            {
                Debug.Log($"player - {p2.id} 还没有队伍");
                return false;
            }
            return p1.team.id == p2.team.id;
        }

        public bool IsSameTeam(int pid1,int pid2)
        {
            foreach (Team team in teams.Values)
            {
                if(team.IsInTeam(pid1) && team.IsInTeam(pid2))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 分配一个队伍
        /// </summary>
        /// <returns></returns>
        public int DistributeOneTeam()
        {
            if (teams.Count < describes.Count) 
            {
                //如果Team数量少于场地上限
                List<TeamDescribe> unuseDescs= describes.Where((TeamDescribe desc) => !teams.ContainsKey(desc.id)).ToList();//在describes中却没在teams中的队伍
                TeamDescribe randomOne = unuseDescs[Random.Range(0, unuseDescs.Count - 1)];//随机一个
                AddTeamByDescribe(randomOne);
                return randomOne.id;
            }
            //如果Team达到上限甚至突破上限（？)
            int minNum = int.MaxValue;
            int minTeamID = -1;
            foreach (Team team in teams.Values) //找一个人最少的队伍
            {
                if (team.Count < minNum)
                {
                    minNum = team.Count;
                    minTeamID = team.id;
                }
            }
            if (minTeamID == -1) //基本不会进这个逻辑
            {
                if (teams.Count == 0)
                {
                    minTeamID = 1;
                }
                else
                {
                    minTeamID = teams.Keys.Max() + 1;
                }
                AddTeamByDescribe(GenerateDescribe(minTeamID));
            }
            return minTeamID;
        }

        public void AddTeamByDescribe(TeamDescribe describe)
        {
            Team team = new Team(describe.id);
            teams.Add(team.id, team);
            team.name = describe.name;
            team.teamColor = describe.color;
        }

        public void TryAddTeamByDescribe(TeamDescribe describe)
        {
            if (!teams.ContainsKey(describe.id))
            {
                AddTeamByDescribe(describe);
            }
        }

        /// <summary>
        /// 尝试通过队伍ID得到队伍描述
        /// </summary>
        /// <param name="teamID">队伍ID</param>
        /// <returns></returns>
        public TeamDescribe GetTeamDescribe(int teamID)
        {
            TeamDescribe? result = null;
            for (int i = 0; i < describes.Count; i++) //找到对应teamID的描述信息
            {
                if (describes[i].id == teamID)
                {
                    result = describes[i];
                    break;
                }
            }
            if (result == null) //如果没找到描述信息，最好不要没找到吧……（一般不走这个逻辑）
            {
                result = GenerateDescribe(teamID);//真的要生成一个新队伍了
            }
            return result.Value;
        }

        //目前的逻辑中，万不得已不用的生成队伍
        public TeamDescribe GenerateDescribe(int teamID)
        {
            if (describes.Count < 2) 
            {
                return new TeamDescribe
                {
                    id = teamID,
                    color = Color.HSVToRGB(Random.Range(0, 1), Random.Range(0.5f, 1), Random.Range(0.5f, 1)),
                    name = GetStuffing() + GetStuffing()
                };
            }
            else
            {
                int dcount = describes.Count - 1;
                TeamDescribe d1 = describes[Random.Range(0, dcount)];
                TeamDescribe d2 = describes[Random.Range(0, dcount)];
                return new TeamDescribe
                {
                    id = teamID,
                    color = Color.Lerp(d1.color, d2.color, Random.Range(0.3f, 0.7f)),
                    name = d1.name + GetStuffing() + d2.name,
                };
            }
        }

        List<string> stuffingList = new List<string> { "上", "如", "加", "传", "入", "转" };
        public string GetStuffing()
        {
            return stuffingList[Random.Range(0, stuffingList.Count - 1)];
        }

    }

}