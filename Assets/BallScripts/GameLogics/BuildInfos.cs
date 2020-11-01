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
    public class PlayerBuildInfo : BaseBuildInfo
    {
        public new StageObjectCategory category = StageObjectCategory.Player;
        public new string infoType = "Player";
        public string playerType = "Demo";
    }

}

