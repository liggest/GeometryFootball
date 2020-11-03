﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;

namespace BallScripts.Servers
{
    public class UltimateCube : BaseUltimate
    {
        Player cubePlayer;
        Vector3 playerPos;
        private float timeUse = 0;
        List<BaseStageObject> objList = new List<BaseStageObject>();
        List<Vector3> initPosList = new List<Vector3> {
            new Vector3(1, 0, 1),
            new Vector3(-1, 0, 1),
            new Vector3(1, 0, -1),
            new Vector3(-1, 0, -1),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
        };

        List<Vector3> endPosList = new List<Vector3> {
            new Vector3(15, 0, 15),
            new Vector3(-15, 0, 15),
            new Vector3(15, 0, -15),
            new Vector3(-15, 0, -15),
            new Vector3(0, 0, 15),
            new Vector3(0, 0, -15),
            new Vector3(15, 0, 0),
            new Vector3(-15, 0, 0),
        };

        public override void Init(Player player)
        {
            cubePlayer = player;
            return;
        }

        public override void FixedUpdate()
        {
            for (int i = 0; i < 8; i++)
            {
                objList[i].transform.position = Vector3.MoveTowards(objList[i].transform.position, playerPos+endPosList[i], 10*Time.fixedDeltaTime);
            }

            Debug.Log("大招位置："+objList[0].transform.position+"判定位置："+playerPos + endPosList[0]+"距离："+Vector3.Distance(objList[0].transform.position, playerPos + endPosList[0]));

            //Vector3 distance = playerPos + endPosList[0] - objList[0].transform.position;

            TimeCount();

            //if (new Vector2(distance.x, distance.z).magnitude < 1f)
            if(timeUse >= 4)
            {
                Exit();
            }

            return;
        }

        public override void Update()
        {

           
            return;
        }

        public override void Enter()
        {
            base.Enter();
            playerPos = cubePlayer.transform.position;
            Vector3 InitPos = playerPos + new Vector3(1, 0, 1);
            RigidBuildInfo info = new RigidBuildInfo
            {
                category = StageObjectCategory.Dynamic,
                id = StageManager.instance.GetMaxID(StageObjectCategory.Dynamic) + 1,
                mass = 50,
                angularDrag = 0.7f,
                prefabName = "RoadBlock",
                position = InitPos,
                rotation = Quaternion.identity,
                //initForce = Vector3.zero,
            };

            for (int i = 0; i < 8; i++)
            {
                RigidBuildInfo newInfo = (RigidBuildInfo)info.Clone();
                newInfo.position = playerPos + initPosList[i];
                newInfo.id += i;
                objList.Add(GameManager.instance.SpawnStageObject(newInfo, BuildType.Server));
                ServerSend.StageObjectSpawned(newInfo);
            }
            //Rigidbody rig = obj.GetComponent<Rigidbody>();
        }

        public override void Exit()
        {
            base.Exit();
            for (int i = 0; i < 8; i++)
            {
                GameManager.instance.DespawnStageObject(objList[i].category, objList[i].id);
            }
            timeUse = 0;
            objList.Clear();
        }

        private void TimeCount()
        {
            timeUse += Time.deltaTime;
        }
    }
}


