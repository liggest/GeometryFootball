using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;

namespace BallScripts.Servers
{
    public class UltimateCube : BaseUltimate
    {
        Player cubePlayer;
        public float blockSpeed = 5f;
        float blockSpeedFactor = 0;
        //Vector3 playerPos;
        //private float timeUse = 0;
        //List<BaseStageObject> objList = new List<BaseStageObject>();
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
            new Vector3(20, 0, 20),
            new Vector3(-20, 0, 20),
            new Vector3(20, 0, -20),
            new Vector3(-20, 0, -20),
            new Vector3(0, 0, 20),
            new Vector3(0, 0, -20),
            new Vector3(20, 0, 0),
            new Vector3(-20, 0, 0),
        };

        public override void Init(Player player)
        {
            cubePlayer = player;
            blockSpeedFactor = blockSpeed * Time.fixedDeltaTime;
            return;
        }

        public override void FixedUpdate()
        {
            /*
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
            */
            return;
        }

        public override void Update()
        {

           
            return;
        }

        public override void Enter()
        {
            base.Enter();
            //playerPos = cubePlayer.transform.position;
            //Vector3 InitPos = playerPos + new Vector3(1, 0, 1);
            RigidBuildInfo info = new RigidBuildInfo
            {
                category = StageObjectCategory.Dynamic,
                id = StageManager.instance.GetMaxID(StageObjectCategory.Dynamic) + 1,
                mass = 50,
                useGravity = false,
                drag = 0f,
                angularDrag = 0f,
                prefabName = "RoadBlock",
                constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ,
                //position = InitPos,
                //rotation = Quaternion.identity,
                //initForce = Vector3.zero,
            };

            Vector3 playerPos = cubePlayer.transform.position;
            List<BaseStageObject> objList = new List<BaseStageObject>();

            for (int i = 0; i < 8; i++)
            {
                RigidBuildInfo newInfo = (RigidBuildInfo)info.Clone();
                newInfo.position = playerPos + initPosList[i];
                newInfo.initForce = initPosList[i] * 5;
                newInfo.initForceMode = ForceMode.VelocityChange;
                newInfo.id += i;
                BaseStageObject obj = GameManager.instance.SpawnStageObject(newInfo, BuildType.Server);
                //obj.transform.forward = initPosList[i].normalized;
                objList.Add(obj);
                ServerSend.StageObjectSpawned(newInfo);
            }
            //Rigidbody rig = obj.GetComponent<Rigidbody>();
            GameManager.instance.StartCoroutine(BlockMove(playerPos, objList)); //不这么写的话关掉游戏携程跑不完，那些块块就不会销毁，所以得挂在一个场景中不会被销毁的东西上
            Exit();
        }

        public override void Exit()
        {
            base.Exit();
            /*
            List <StageObjectPair> deSpawnObjList = new List<StageObjectPair>();
            for (int i = 0; i < 8; i++)
            {
                GameManager.instance.DespawnStageObject(objList[i].category, objList[i].id);
                deSpawnObjList.Add(new StageObjectPair {category = objList[i].category, id = objList[i].id });
            }
            ServerSend.StageObjectDespawned(deSpawnObjList);
            timeUse = 0;
            objList.Clear();
            */
        }
        /*
        private void TimeCount()
        {
            timeUse += Time.deltaTime;
        }
        */
        IEnumerator BlockMove(Vector3 playerPos,List<BaseStageObject> objList)
        {
            float timeUse = 0;

            while (timeUse < 4)
            {
                /*
                for (int i = 0; i < 8; i++)
                {
                    //objList[i].transform.position = Vector3.MoveTowards(objList[i].transform.position, playerPos + endPosList[i], 10 * Time.fixedDeltaTime);
                    objList[i].transform.position += objList[i].transform.forward * blockSpeedFactor;
                    //objList[i].transform.Translate(objList[i].transform.forward * blockSpeedFactor);
                    //Debug.Log("大招位置：" + objList[0].transform.position + "判定位置：" + playerPos + endPosList[0] + "距离：" + Vector3.Distance(objList[0].transform.position, playerPos + endPosList[0]));
                }
                */
                timeUse += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            List<StageObjectPair> despawnObjList = new List<StageObjectPair>();
            for (int i = 0; i < 8; i++)
            {
                GameManager.instance.DespawnStageObject(objList[i].category, objList[i].id);
                despawnObjList.Add(new StageObjectPair { category = objList[i].category, id = objList[i].id });
            }
            ServerSend.StageObjectDespawned(despawnObjList);
            Debug.Log($"大招结束，用时{timeUse}");
        }
    }
}


