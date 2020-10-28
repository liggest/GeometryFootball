using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine;

namespace BallScripts.GameLogics
{
    public class ResourcesManager
    {
        //public static Dictionary<string, GameObject> playerPrefabs = new Dictionary<string, GameObject>();
        //public static Dictionary<string, GameObject> ballPrefabs = new Dictionary<string, GameObject>();

        public static Dictionary<string, Dictionary<string, GameObject>> prefabs = new Dictionary<string, Dictionary<string, GameObject>>();

        public static List<string> labels = new List<string> { "Players", "Balls" };

        public static void Init()
        {
            for (int i = 0; i < labels.Count; i++)
            {
                prefabs.Add(labels[i], new Dictionary<string, GameObject>());
            }
        }

        public static void Load()
        {
            for (int i = 0; i < labels.Count; i++)
            {
                LoadByLabel(labels[i]);
            }
        }

        public static void LoadByLabel(string label)
        {
            Addressables.LoadAssetsAsync<GameObject>(label, null).Completed +=
                (AsyncOperationHandle<IList<GameObject>> objlist) =>
                {
                    foreach (GameObject obj in objlist.Result)
                    {
                        prefabs[label].Add(obj.name, obj);
                    }
                    Debug.Log($"[ResourcesManager]加载了{label}");
                };
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

