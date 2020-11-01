using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;

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
                    obj.gameObject.AddComponent<InfoSender>();
                }
            }
        }

        public static void InitClientPlayer(int clientID,string prefabName)
        {
            PlayerBuildInfo info = new PlayerBuildInfo
            {
                id = clientID,
                prefabName = prefabName,
                playerType = "Demo",
                firstBar = StageManager.instance.GetMaxID(StageObjectCategory.Dynamic) + 1,
            };
            Debug.Log($"info.firstBar={info.firstBar}");
            GameManager.instance.SpawnStageObject(info, BuildType.Server);
            ServerSend.StageObjectSpawned(info);
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
    }
}


