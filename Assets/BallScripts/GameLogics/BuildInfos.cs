using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;


namespace BallScripts.GameLogics
{
    [Serializable]
    public class BaseBuildInfo
    {
        public StageObjectCategory category = StageObjectCategory.Other;
        public int id = -1;
        public string prefabName = "";
        public string infoType = "Base";
    }

    [Serializable]
    public class RigidBuildInfo:BaseBuildInfo
    {
        public new StageObjectCategory category = StageObjectCategory.Dynamic;
        public new string infoType = "Rigid";
        public CollisionDetectionMode collisionMode = CollisionDetectionMode.Discrete;
        public float mass = 1;
        public float angularDrag = 0.05f;
    }

    [Serializable]
    public class PlayerBuildInfo : BaseBuildInfo
    {
        public new StageObjectCategory category = StageObjectCategory.Player;
        public new string infoType = "Player";
        public string playerType = "Demo";
        public int firstBar = -1;
    }


}

