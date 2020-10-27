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

        public static void InitDemo()
        {
            Rigidbody rig = StageManager.instance.GetStageObject(StageObjectCategory.Ball, 1).gameObject.AddComponent<Rigidbody>();
            rig.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }
}


