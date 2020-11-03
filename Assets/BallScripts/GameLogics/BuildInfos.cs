using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;


namespace BallScripts.GameLogics
{
    [Serializable]
    public class BaseBuildInfo: ICloneable
    {
        public StageObjectCategory category = StageObjectCategory.Other;
        public int id = -1;
        public string prefabName = string.Empty;
        public string infoType = "Base";
        public Vector3? position = null;
        public Quaternion? rotation = null;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Serializable]
    public class RigidBuildInfo:BaseBuildInfo
    {
        public RigidBuildInfo()
        {
            category= StageObjectCategory.Dynamic;
            infoType= "Rigid";
        }
        public CollisionDetectionMode collisionMode = CollisionDetectionMode.Discrete;
        public float mass = 1;
        public float angularDrag = 0.05f;
        public Vector3? initForce = null;
        public ForceMode initForceMode = ForceMode.Force;
    }

    [Serializable]
    public class PlayerBuildInfo : BaseBuildInfo
    {
        public PlayerBuildInfo()
        {
            category = StageObjectCategory.Player;
            infoType = "Player";
        }
        public string playerType = "Demo";
        public int firstBar = -1;
    }


}

