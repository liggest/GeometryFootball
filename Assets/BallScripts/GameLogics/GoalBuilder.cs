using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.GameLogics
{
    public class GoalBuilder : BaseBuilder<Goal, GoalBuildInfo>
    {
        public override Goal BuildInClient(Goal obj, GoalBuildInfo info)
        {
            if (TeamManager.instance.teams.TryGetValue(info.teamID, out Team team)) 
            {
                team.AddGoal(obj);
            }
            else
            {
                TeamManager.instance.CacheGoal(info.teamID, obj);//如果没找到队伍，先把球门存起来
            }
            return obj;
        }

        public override Goal BuildInServer(Goal obj, GoalBuildInfo info)
        {
            if (TeamManager.instance.teams.TryGetValue(info.teamID, out Team team)) 
            {
                team.AddGoal(obj);
            }
            return obj;
        }

        public override Goal BuildObject(GoalBuildInfo info)
        {
            if (Goal.unbindGoals.Count > 0 && !string.IsNullOrEmpty(info.goalName))  
            {
                if(Goal.unbindGoals.TryGetValue(info.goalName,out Goal goal))
                {
                    goal.Init(info.category, info.id);
                    goal.builder = this;
                    return goal;
                }
            }
            else if (!string.IsNullOrEmpty(info.prefabName))
            {
                return base.BuildObject(info);
            }
            return null;
        }

        public override bool CheckInfo(BaseBuildInfo info)
        {
            return info is GoalBuildInfo;
        }

        public override GoalBuildInfo GenerateInfo(Goal obj)
        {
            GoalBuildInfo info = new GoalBuildInfo
            {
                teamID = obj.TeamID,
                goalName = obj.name
            };
            return SetBaseInfo(info, obj);
        }
    }
}

