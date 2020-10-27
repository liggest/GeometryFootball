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

        public static void AttachInfoSenders()
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

        public static void AttachPlayerController()
        {

        }

        public static void AttachRigidbody(StageObjectCategory category)
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


