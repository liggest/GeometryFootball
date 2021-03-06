﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BallScripts.Servers;


namespace BallScripts.GameLogics
{
    [Serializable]
    public class BaseBuildInfo : ICloneable
    {
        public StageObjectCategory category = StageObjectCategory.Other;
        public int id = -1;
        public string prefabName = string.Empty;
        public string infoType = "Base";
        public Vector3? position = null;
        public Quaternion? rotation = null;
        public SendFlag sendFlags = SendFlag.Position | SendFlag.Rotation;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Serializable]
    public class RigidBuildInfo : BaseBuildInfo
    {
        public RigidBuildInfo()
        {
            category= StageObjectCategory.Dynamic;
            infoType= "Rigid";
        }
        public float mass = 1;
        public float drag = 0;
        public float angularDrag = 0.05f;
        public bool useGravity = true;
        public bool isKinematic = false;
        public CollisionDetectionMode collisionMode = CollisionDetectionMode.Discrete;
        public RigidbodyConstraints constraints = RigidbodyConstraints.None;
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
        public string playerName = "Anonymous";
        public int firstBar = -1;
        public TeamDescribe teamDescribe = new TeamDescribe { id = -1 };
        public int score = 0;
    }

    [Serializable]
    public class GoalBuildInfo : BaseBuildInfo
    {
        public GoalBuildInfo()
        {
            category = StageObjectCategory.Goal;
            infoType = "Goal";
        }
        public string goalName = string.Empty;
        public int teamID = -1;
        public int score = 0;
    }
}

