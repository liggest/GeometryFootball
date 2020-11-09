using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BallScripts.GameLogics
{
    public class PanelBase : MonoBehaviour
    {
        Dictionary<string, List<UIBehaviour>> dic = new Dictionary<string, List<UIBehaviour>>();
        void Awake()
        {
            GetUIComponent<Button>();
            UIManager.instance.AddToMainDic(transform.name, dic);
        }
        
        /// <summary>
        /// 获取该类型的所有组件，添加到字典中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void GetUIComponent<T>() where T : UIBehaviour
        {
            if (dic == null)
            {
                return;
            }
            T[] ts = GetComponentsInChildren<T>();
            for (int i = 0; i < ts.Length; i++)
            {
                if (!dic.ContainsKey(ts[i].gameObject.name))
                {
                    dic.Add(ts[i].gameObject.name, new List<UIBehaviour>());
                }
                dic[ts[i].gameObject.name].Add(ts[i]);
            }
        }
    }


}

