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
            PlayerBuilder pb = new PlayerBuilder();
            Player pl = pb.Build(prefabName, BuildType.Server);
            pl.Init(StageObjectCategory.Player, clientID);
            Debug.Log("服务端初始化Player");
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


