using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BallScripts.GameLogics;
using BallScripts.Servers;

namespace BallScripts.Servers
{
    public class ServerLogic
    {
        public static void InitInfoBuffer()
        {
            var infoBufferObject = new GameObject();
            infoBufferObject.name = "InfoBuffer";
            infoBufferObject.AddComponent<InfoBuffer>();
            Object.DontDestroyOnLoad(infoBufferObject);
        }

        public static void AttachInfoSendersToAll()
        {
            foreach(StageObjectCategory category in StageManager.instance.stageObjects.Keys)
            {
                if (category == StageObjectCategory.Static)
                {
                    continue;
                }
                foreach(BaseStageObject obj in StageManager.instance.stageObjects[category].Values)
                {
                    if(!obj.TryGetComponent(out InfoSender sender))
                    {
                        obj.gameObject.AddComponent<InfoSender>();
                    }
                }
            }
        }

        public static void InitClientPlayer(int clientID,string prefabName)
        {
            int teamID = TeamManager.instance.DistributeOneTeam();
            Team team = TeamManager.instance.teams[teamID];
            TeamDescribe desc = team.GetDescribe();
            if (team.HasNoGoal)
            {
                Goal goal = TeamManager.instance.DistributeOneGoal();
                GoalBuildInfo goalInfo = new GoalBuildInfo
                {
                    id = StageManager.instance.GetMaxID(StageObjectCategory.Goal) + 1,
                    teamID = team.id,
                    goalName = goal.name,
                    score = 0
                };
                GameManager.instance.SpawnStageObject(goalInfo, BuildType.Server);
                ServerSend.StageObjectSpawned(goalInfo);
            }

            Vector3 playerPos = team.GetSpawnPoint();
            Quaternion playerRot = Quaternion.LookRotation(Vector3.zero - new Vector3(playerPos.x, 0, playerPos.z));
            PlayerBuildInfo info = new PlayerBuildInfo
            {
                id = clientID,
                prefabName = prefabName,
                playerType = "Demo",
                playerName = Server.clients[clientID].username,
                firstBar = StageManager.instance.GetMaxID(StageObjectCategory.Dynamic) + 1,
                teamDescribe = desc,
                score = 0,
                position = playerPos,
                rotation = playerRot
            };
            GameManager.instance.SpawnStageObject(info, BuildType.Server);
            ServerSend.StageObjectSpawned(info);
        }

        public static void PlayerDisconnected(int playerID)
        {
            BaseStageObject obj = StageManager.instance.GetStageObject(StageObjectCategory.Player, playerID);
            if (obj)
            {
                Player player = (Player)obj;
                TeamManager.instance.RemoveFromTeam(player);
                ServerSend.TeamLeft(playerID);
                List<StageObjectPair> bars = new List<StageObjectPair>();
                player.barList.ForEach((Bar bar) =>
                {
                    bars.Add(new StageObjectPair { category = bar.category, id = bar.id });
                    StageManager.instance.RemoveStageObject(bar.category, bar.id);
                });
                ServerSend.StageObjectRemoved(bars);
                GameManager.instance.DespawnStageObject(StageObjectCategory.Player, playerID);
                ServerSend.StageObjectDespawned(StageObjectCategory.Player, playerID);
            }
        }

        public static void InitBall(string prefabName)
        {
            RigidBuildInfo info = new RigidBuildInfo
            {
                category = StageObjectCategory.Ball,
                id = StageManager.instance.GetMaxID(StageObjectCategory.Ball) + 1,
                prefabName = prefabName,
                collisionMode = CollisionDetectionMode.Continuous,
                drag = 0.5f, //空气阻力，用起来很赞
                //position 需要初始位置
            };
            GameManager.instance.SpawnStageObject(info, BuildType.Server);
            //ServerSend.StageObjectSpawned(info);
        }


        public static void AttachRigidbodyToAll(StageObjectCategory category)
        {
            foreach (BaseStageObject obj in StageManager.instance.stageObjects[category].Values)
            {
                Rigidbody rig = obj.gameObject.AddComponent<Rigidbody>();
                rig.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
        }

        public static void InitDemo()
        {
            
        }

        public static void RefreshBeforeLoad()
        {
            PlayerController.ResetLast();
        }

        public static void StageSynchronize(int clientID, List<StageObjectPair> clientObjs)
        {
            Debug.Log($"收到了 客户端{clientID} 发来的场景同步请求");
            List<StageObjectPair> serverObjs = StageManager.instance.GetStageObjectPairs(new List<StageObjectPair> {
               new StageObjectPair { category = StageObjectCategory.Player, id = clientID }
            }); //得到除去该客户端Player以外的元素
            IEnumerable<StageObjectPair> intersection = serverObjs.Intersect(clientObjs);
            IEnumerable<StageObjectPair> serverOnly = serverObjs.Except(intersection);
            IEnumerable<StageObjectPair> clientOnly = clientObjs.Except(intersection);
            Debug.Log($"当前：{serverObjs.Count},客户端：{clientObjs.Count},交集：{intersection.Count()},服务器独有：{serverOnly.Count()},客户端独有：{clientOnly.Count()}");
            List<Vector3Holder> posList = new List<Vector3Holder>();
            List<QuaternionHolder> rotList = new List<QuaternionHolder>();
            foreach (StageObjectPair pair in intersection) 
            {
                BaseStageObject obj = StageManager.instance.GetStageObject(pair.category, pair.id);
                posList.Add(new Vector3Holder { category = pair.category, id = pair.id, vect = obj.transform.position });
                rotList.Add(new QuaternionHolder { category = pair.category, id = pair.id, quat = obj.transform.rotation });
            }
            ServerSend.StageObjectPositions(clientID, posList);
            ServerSend.StageObjectRotations(clientID, rotList);
            ServerSend.StageObjectDespawned(clientID, clientOnly.ToList());
            foreach (StageObjectPair pair in serverOnly)
            {
                BaseStageObject obj = StageManager.instance.GetStageObject(pair.category, pair.id);
                if (obj.builder != null)
                {
                    BaseBuildInfo info = obj.builder.GenerateBuildInfo(obj);
                    if (info != null)
                    {
                        ServerSend.StageObjectSpawned(clientID, info);
                    }
                }
            }
        }
    }
}


