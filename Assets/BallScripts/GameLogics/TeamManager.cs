using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BallScripts.Utils;

namespace BallScripts.GameLogics{

    [System.Serializable]
    public struct TeamDescribe
    {
        public Color color;
        public string name;
    }

    public class TeamManager : Singleton<TeamManager>
    {
        public Dictionary<int, Team> teams = new Dictionary<int, Team>();

        public List<TeamDescribe> describes = new List<TeamDescribe>();

        public void AddToTeam(Player player,int teamID)
        {
            if (teams.TryGetValue(teamID, out Team team))
            {
                team.Add(player.id);
                player.team = team;
            }
            else
            {
                Team playerTeam = new Team(teamID);
                teams.Add(teamID, playerTeam);
                TeamDescribe desc = GetTeamDescribe(playerTeam.id);
                playerTeam.name = desc.name;
                playerTeam.teamColor = desc.color;
                playerTeam.Add(player);
                player.team = playerTeam;
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
            bool result = false;
            foreach (Team team in teams.Values)
            {
                result = team.IsInTeam(pid1) && team.IsInTeam(pid2);
            }
            return result;
        }

        public int DistributeToTeam(Player player)
        {
            int minNum = int.MaxValue;
            int minTeamID = -1;
            foreach (Team team in teams.Values)
            {
                if (team.Count < minNum)
                {
                    minNum = team.Count;
                    minTeamID = team.id;
                }
            }
            if (minTeamID == -1)
            {
                AddToTeam(player, teams.Keys.Max() + 1);
            }
            else
            {
                AddToTeam(player, minTeamID);
            }
            return minTeamID;
        }

        public TeamDescribe GetTeamDescribe(int teamID)
        {
            if(teamID>0 && teamID < describes.Count)
            {
                return describes[teamID];
            }
            else
            {
                List<string> centerList = new List<string> { "上", "如", "加", "传" };
                string center = centerList[Random.Range(0, centerList.Count - 1)];
                int dcount = describes.Count - 1;
                TeamDescribe d1 = describes[Random.Range(0, dcount)];
                TeamDescribe d2 = describes[Random.Range(0, dcount)];
                return new TeamDescribe
                {
                    color = Color.Lerp(d1.color, d2.color, Random.Range(0.3f, 0.7f)),
                    name = d1.name + center + d2.name,
                };
            }
        }

    }

}