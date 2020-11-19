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
            rig.mass = info.mass;
            rig.drag = info.drag;
            rig.angularDrag = info.angularDrag;
            rig.useGravity = info.useGravity;
            rig.isKinematic = info.isKinematic;
            rig.collisionDetectionMode = info.collisionMode;
            rig.constraints = info.constraints;
            obj.gameObject.AddComponent<InfoSender>().sendFlags = info.sendFlags;
            if (info.initForce != null) 
            {
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    //Debug.Log($"加了{info.initForce.Value}力，mode={info.initForceMode}");
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
                drag = rig.drag,
                angularDrag = rig.angularDrag,
                useGravity = rig.useGravity,
                isKinematic = rig.isKinematic,
                collisionMode = rig.collisionDetectionMode,
                constraints = rig.constraints
            };
            return SetBaseInfo(info, obj);
        }
    }
}

