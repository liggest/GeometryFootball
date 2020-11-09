using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallScripts.GameLogics {
    public class Team
    {
        public int id = -1;
        HashSet<int> playerIDs = new HashSet<int>();
        public string name = string.Empty;
        public Color teamColor = Color.black;
        public Team(int teamID)
        {
            id = teamID;
        }
        public Team(TeamDescribe describe)
        {
            id = describe.id;
            name = describe.name;
            teamColor = describe.color;
        }

        public int Count { get => playerIDs.Count; }

        public bool HasNoGoal { get => goals.Count == 0; }

        List<Goal> goals = new List<Goal>();

        public void Add(int playerID)
        {
            if (IsInTeam(playerID))
            {
                Debug.Log($"player - {playerID} 已经在队伍 {id} 中");
            }
            else
            {
                playerIDs.Add(playerID);
            }
        }
        public void Add(Player player)
        {
            Add(player.id);
        }

        public bool IsInTeam(int playerID)
        {
            return playerIDs.Contains(playerID);
        }
        public bool IsInTeam(Player player)
        {
            return IsInTeam(player.id);
        }

        public void Remove(int playerID)
        {
            playerIDs.Remove(playerID);
        }
        public void Remove(Player player)
        {
            Remove(player.id);
        }

        public void Clear()
        {
            playerIDs.Clear();
        }

        public int GetRandom()
        {
            int[] players = GetArray();
            return players[Random.Range(0, players.Length - 1)];
        }

        public int[] GetArray()
        {
            return playerIDs.ToArray();
        }

        public List<int> GetList()
        {
            return playerIDs.ToList();
;       }

        public TeamDescribe GetDescribe()
        {
            return new TeamDescribe
            {
                id = id,
                color = teamColor,
                name = name
            };
        }
        public override string ToString()
        {
            return $"队伍{id}-{name}队";
        }

        public void AddGoal(Goal goal)
        {
            goals.Add(goal);
            goal.BindToTeam(this);
        }

        public void RemoveGoal(Goal goal)
        {
            if (goals.Remove(goal))
            {
                goal.UnbindFromTeam();
            }
        }

        public void RemoveGoal(int id)
        {
            for (int i = 0; i < goals.Count; i++)
            {
                if (goals[i].id == id)
                {
                    RemoveGoal(goals[i]);
                    break;
                }
            }
        }

        public void RemoveAllGoals()
        {
            while (goals.Count > 0)
            {
                RemoveGoal(goals[0]);
            }
        }

        private int score = 0;
        public int Score { get => score; set => score = value > 0 ? value : 0; }

        public void AddSocre(int value)
        {
            Score += value;
            Actions.TeamScoredAction?.Invoke(this, value);
        }

        public void ResetScore()
        {
            Score = 0;
        }
    }
}

