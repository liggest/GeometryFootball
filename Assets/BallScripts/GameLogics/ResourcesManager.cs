using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine;
using System.Threading.Tasks;

namespace BallScripts.GameLogics
{
    public class ResourcesManager
    {
        //public static Dictionary<string, GameObject> playerPrefabs = new Dictionary<string, GameObject>();
        //public static Dictionary<string, GameObject> ballPrefabs = new Dictionary<string, GameObject>();

        //public static Dictionary<string, Dictionary<string, GameObject>> prefabs = new Dictionary<string, Dictionary<string, GameObject>>();

        //public static Dictionary<string, Hashtable> resources = new Dictionary<string, Hashtable>();

        public static Hashtable resources = new Hashtable();

        public static List<string> labels = new List<string> { "Players", "Balls", "Dynamic" };

        public static void LoadAll()
        {
            for (int i = 0; i < labels.Count; i++)
            {
                LoadByLabel<GameObject>(labels[i]);
            }
        }

        public static async void LoadByLabel<T>(string label, Action<T> onLoad = null) where T : UnityEngine.Object
        {
            IList<T> resultList = await Addressables.LoadAssetsAsync<T>(label, null).Task;

            foreach (T obj in resultList)
            {
                resources[obj.name] = obj;
                onLoad?.Invoke(obj);
            }
            Debug.Log($"[ResourcesManager]加载了所有{label}，共{resultList.Count}个。目前共有{resources.Count}个资源");
        }

        public static async void Load<T>(string name, Action<T> onLoad = null)
        {
            T result = await Addressables.LoadAssetAsync<T>(name).Task;
            resources[name] = result;
            onLoad?.Invoke(result);
            Debug.Log($"[ResourcesManager]加载了{name}");
        }

        public static async void Load<T>(string name, string label, Action<T> onLoad = null)
        {
            IList<T> result = await Addressables.LoadAssetsAsync<T>(new List<object> { name, label }, null, Addressables.MergeMode.Intersection).Task;
            T obj = result[0];
            resources[name] = obj;
            onLoad?.Invoke(obj);
            Debug.Log($"[ResourcesManager]加载了{label}下的{name}");
        }

        //Transform parent = null, Vector3? position = null, Quaternion? rotation = null
        public static async void LoadAndInstantiate(string name, Transform parent = null, Action<GameObject> onLoad = null)
        {
            GameObject obj = await Addressables.InstantiateAsync(name, parent).Task;
            onLoad?.Invoke(obj);
            Debug.Log($"[ResourcesManager]生成了{name}");
        }
        public static async void LoadAndInstantiate(string name, Vector3 position, Quaternion rotation, Transform parent = null, Action<GameObject> onLoad = null)
        {
            GameObject obj = await Addressables.InstantiateAsync(name, position, rotation, parent).Task;
            onLoad?.Invoke(obj);
            Debug.Log($"[ResourcesManager]生成了{name}");
        }

        public static T Get<T>(string name)
        {
            if (resources.ContainsKey(name))
            {
                return (T)resources[name];
            }
            return default;
        }

        public static void Release<T>(string name)
        {
            T obj = Get<T>(name);
            if (obj != null)
            {
                Addressables.Release(obj);
                resources.Remove(name);
            }
        }

        /*
        public static void LoadPlayers()
        {
            //Addressables.LoadAssetsAsync<GameObject>(new AssetLabelReference() { labelString = "Players" }, null).Completed +=
            Addressables.LoadAssetsAsync<GameObject>("Players", null).Completed +=
                (AsyncOperationHandle<IList<GameObject>> playerlist) =>
                {
                    foreach (GameObject obj in playerlist.Result)
                    {
                        playerPrefabs.Add(obj.name, obj);
                    }
                    Debug.Log("[ResourcesManager]加载了Players");
                };
        }

        public static void LoadBalls()
        {
            Addressables.LoadAssetsAsync<GameObject>("Balls", null).Completed +=
                (AsyncOperationHandle<IList<GameObject>> balllist) =>
                {
                    foreach (GameObject obj in balllist.Result)
                    {
                        ballPrefabs.Add(obj.name, obj);
                    }
                    Debug.Log("[ResourcesManager]加载了Balls");
                };
        }
        */
    }

}

