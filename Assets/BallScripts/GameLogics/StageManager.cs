using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Utils;
using System.Linq;

namespace BallScripts.GameLogics {

    public struct StageObjectPair
    {
        public StageObjectCategory category;
        public int id;
    }

    public class StageManager : Singleton<StageManager>
    {
        public Dictionary<StageObjectCategory, Dictionary<int, BaseStageObject>> stageObjects;
        private Dictionary<StageObjectCategory, int> maxIDs;
        Type categoryType = typeof(StageObjectCategory);

        private void Awake()
        {
            InitDicts();
        }

        public void InitDicts()
        {
            stageObjects = new Dictionary<StageObjectCategory, Dictionary<int, BaseStageObject>>();
            maxIDs = new Dictionary<StageObjectCategory, int>();
            foreach (StageObjectCategory category in Enum.GetValues(categoryType))
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

        public BaseStageObject RemoveStageObject(StageObjectCategory category,int id)
        {
            BaseStageObject obj = GetStageObject(category, id);
            if (obj)
            {
                stageObjects[category].Remove(id);
                if (maxIDs[category] == id)
                {
                    maxIDs[category]--; //或许能让maxid增长慢一点，估计没啥效果
                }
            }
            return obj;
        }

        public int GetMaxID(StageObjectCategory category)
        {
            return maxIDs[category];
        }

        public List<BaseStageObject> GetSceneObjects()
        {
            return new List<BaseStageObject>(FindObjectsOfType<BaseStageObject>());
        }

        public List<StageObjectPair> GetStageObjectPairs()
        {
            List<StageObjectPair> pairs = new List<StageObjectPair>();
            foreach (StageObjectCategory category in Enum.GetValues(categoryType))
            {
                foreach (int id in stageObjects[category].Keys)
                {
                    pairs.Add(new StageObjectPair { category = category, id = id });
                }
            }
            return pairs;
        }
        public List<StageObjectPair> GetStageObjectPairs(List<StageObjectPair> excepts)
        {
            HashSet<StageObjectPair> pairs = new HashSet<StageObjectPair>(GetStageObjectPairs());
            pairs.ExceptWith(excepts);
            return pairs.ToList();
        }

    }

}
