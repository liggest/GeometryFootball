using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Servers;
using BallScripts.Utils;

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
            if (info.initForce != null) 
            {
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    rig.AddForce(info.initForce.Value, info.initForceMode);
                });
            }
            return obj;
        }

        public override bool CheckInfo(BaseBuildInfo info)
        {
            return info is RigidBuildInfo;
        }

        public override RigidBuildInfo GenerateInfo(BaseStageObject obj)
        {
            Rigidbody rig = obj.GetComponent<Rigidbody>();
            RigidBuildInfo info = new RigidBuildInfo
            {
                mass = rig.mass,
                angularDrag = rig.angularDrag,
                collisionMode = rig.collisionDetectionMode
            };
            return SetBaseInfo(info, obj);
        }
    }
}

