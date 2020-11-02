using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Servers;

namespace BallScripts.GameLogics
{
    public class RigidBuilder : BaseBuilder<BaseStageObject, RigidBuildInfo>
    {
        public override BaseStageObject BuildInClient(BaseStageObject obj, RigidBuildInfo info)
        {
            return obj;
        }

        public override BaseStageObject BuildInServer(BaseStageObject obj, RigidBuildInfo info)
        {
            Rigidbody rig = obj.gameObject.AddComponent<Rigidbody>();
            rig.collisionDetectionMode = info.collisionMode;
            rig.mass = info.mass;
            rig.angularDrag = info.angularDrag;
            obj.gameObject.AddComponent<InfoSender>();
            return obj;
        }

        public override BaseStageObject BuildObject(RigidBuildInfo info)
        {
            GameObject RigidPrefab = ResourcesManager.Get<GameObject>(info.prefabName);
            if (RigidPrefab)
            {
                GameObject rigider = Object.Instantiate(RigidPrefab);
                BaseStageObject rg = rigider.GetComponent<BaseStageObject>();
                rg.Init(info.category, info.id);
                return rg;
            }
            else
            {
                Debug.Log($"没有找到名为{info.prefabName}的Prefab");
            }
            return null;
        }

        public override bool CheckInfo(BaseBuildInfo info)
        {
            return info is RigidBuildInfo;
        }
    }
}

