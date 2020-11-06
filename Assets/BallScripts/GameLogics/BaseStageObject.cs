using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.GameLogics
{
    public enum StageObjectCategory {
        Player=1,
        Ball,
        Goal,
        Dynamic,
        Static,
        Other
    }

    public class BaseStageObject : MonoBehaviour
    {
        public StageObjectCategory category = StageObjectCategory.Other;
        [Tooltip("id>0则代表物体开局就出现在场景中\n请保证场景中任意两个物体的Category和id至少有一个不同")]
        public int id = -1;
        [Tooltip("开启后场景中的物体，载入时id会忽略默认值，从1自增")]
        public bool idSelfIncrease = false;
        [HideInInspector]
        public string prefabName = string.Empty;
        [HideInInspector]
        public IBuilderNode builder;
        bool isInited = false;

        protected void Start()
        {
            if (idSelfIncrease)
            {
                id = StageManager.instance.GetMaxID(category) + 1;
                Init();
            }
            else if (id > 0)
            {
                Init();
            }
        }

        public void Init()
        {
            if (isInited)
            {
                return;
            }
            StageManager.instance.AddStageObject(category, id, this);
            isInited = true;
        }

        public void Init(StageObjectCategory _category,int _id)
        {
            category = _category;
            id = _id;
            Init();
        }

        public void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        public void SetLocalPosition(Vector3 pos)
        {
            transform.localPosition = pos;
        }

        public void SetRotation(Quaternion rot)
        {
            transform.rotation = rot;
        }

        public void SetLocalRotation(Quaternion rot)
        {
            transform.localRotation = rot;
        }

        public virtual void LastWord()
        {
            Debug.Log($"{category} - {id} 在说遗言");
        }
    }

}