using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;

namespace BallScripts.Servers
{
    public class UltimateCube : BaseUltimate
    {
        public override void Init(Player player)
        {
            return;
        }

        public override void FixedUpdate()
        {
            return;
        }

        public override void Update()
        {
            return;
        }

        public override void Enter()
        {
            base.Enter();
            RigidBuildInfo info = new RigidBuildInfo
            {
                category = StageObjectCategory.Dynamic,
                id = StageManager.instance.GetMaxID(StageObjectCategory.Dynamic) + 1,
                mass = 100,
                angularDrag = 0.7f,
                prefabName = "RoadBlock"
            };
            BaseStageObject obj = GameManager.instance.SpawnStageObject(info, BuildType.Server);
            ServerSend.StageObjectSpawned(info);
            Rigidbody rig = obj.GetComponent<Rigidbody>();
            rig.AddForce(new Vector3(3, 0, 1));
            Exit();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}


