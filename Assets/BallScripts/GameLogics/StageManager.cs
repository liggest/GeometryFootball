using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Utils;

namespace BallScripts.GameLogics {

    public class StageManager : Singleton<StageManager>
    {
        public Dictionary<StageObjectCategory, Dictionary<int, BaseStageObject>> stageObjects;
        private Dictionary<StageObjectCategory, int> maxIDs;

        private void Awake()
        {
            InitDicts();
        }

        public void InitDicts()
        {
            stageObjects = new Dictionary<StageObjectCategory, Dictionary<int, BaseStageObject>>();
            maxIDs = new Dictionary<StageObjectCategory, int>();
            foreach (StageObjectCategory category in Enum.GetValues(typeof(StageObjectCategory)))
            {
                stageObjects.Add(category, new Dictionary<int, BaseStageObject>());
                maxIDs.Add(category, 0);
            }
        }

        public void AddStageObject(StageObjectCategory category, int id, BaseStageObject obj)
        {
            stageObjects[category].Add(id, obj);
            if (id > maxIDs[category]) 
            {
                maxIDs[category] = id;
            }
        }

        public BaseStageObject GetStageObject(StageObjectCategory category, int id)
        {
            if(stageObjects[category].TryGetValue(id,out BaseStageObject obj))
            {
                return obj;
            }
            return null;
        }

        public int GetMaxID(StageObjectCategory category)
        {
            return maxIDs[category];
        }

        public List<BaseStageObject> GetSceneObjects()
        {
            return new List<BaseStageObject>(FindObjectsOfType<BaseStageObject>());
        }

    }

}
