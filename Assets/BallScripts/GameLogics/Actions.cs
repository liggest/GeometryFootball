using System;

namespace BallScripts.GameLogics
{
    class Actions
    {
#pragma warning disable 0649
        //#pragma去除警告
        /// <summary>
        /// 队伍得分时触发 Team是得分队伍、int是分数变化值
        /// </summary>
        public static Action<Team, int> TeamScoredAction;

#pragma warning restore 0649
    }
}