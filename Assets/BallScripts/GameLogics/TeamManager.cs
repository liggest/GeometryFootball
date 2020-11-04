using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Utils;

namespace BallScripts.GameLogics{
    public class TeamManager : Singleton<TeamManager>
    {
        public Dictionary<int, Team> teams = new Dictionary<int, Team>();

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

    }

}