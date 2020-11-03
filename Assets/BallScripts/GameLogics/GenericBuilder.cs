using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Servers;

namespace BallScripts.GameLogics
{
    public class GenericBuilder : BaseBuilder<BaseStageObject, BaseBuildInfo>
    {

        public override BaseStageObject BuildInClient(BaseStageObject obj, BaseBuildInfo info)
        {
            return obj;
        }

        public override BaseStageObject BuildInServer(BaseStageObject obj, BaseBuildInfo info)
        {
            if (info.category != StageObjectCategory.Static)
            {
                obj.gameObject.AddComponent<InfoSender>();
            }
            return obj;
        }

        public override bool CheckInfo(BaseBuildInfo info)
        {
            return info.GetType() == typeof(BaseBuildInfo);
        }

        public override BaseBuildInfo GenerateInfo(BaseStageObject obj)
        {
            return SetBaseInfo(new BaseBuildInfo(), obj);
        }
    }
}


