using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BallScripts.GameLogics {
    public class Team
    {
        public int id = -1;
        HashSet<int> playerIDs = new HashSet<int>();

        public Team(int teamID)
        {
            id = teamID;
        }

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
    }
}

