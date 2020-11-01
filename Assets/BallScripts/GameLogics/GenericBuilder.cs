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

        public override BaseStageObject BuildObject(BaseBuildInfo info)
        {
            GameObject prefab = ResourcesManager.Get<GameObject>(info.prefabName);
            if (prefab)
            {
                GameObject obj = Object.Instantiate(prefab);
                BaseStageObject stageObject = obj.GetComponent<BaseStageObject>();
                stageObject.category = info.category;
                stageObject.id = info.id;
                return stageObject;
            }
            else
            {
                Debug.Log($"没有找到名为{info.prefabName}的Prefab");
            }
            return null;
        }

        public override bool CheckInfo(BaseBuildInfo info)
        {
            return info.GetType() == typeof(BaseBuildInfo);
        }
    }
}


