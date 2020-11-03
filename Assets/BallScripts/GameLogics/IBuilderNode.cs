using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.GameLogics
{
    public interface IBuilderNode
    {
        IBuilderNode Next { get; set; }
        bool CheckInfo(BaseBuildInfo info);

        IBuilderNode GetCorrectBuilder(BaseBuildInfo info);

        BaseStageObject Build(BaseBuildInfo info, BuildType type = BuildType.Client);

        BaseBuildInfo GenerateBuildInfo(BaseStageObject obj);
    }
}


