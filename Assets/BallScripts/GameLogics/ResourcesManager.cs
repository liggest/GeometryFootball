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
        public static Dictionary<string, GameObject> playerPrefabs = new Dictionary<string, GameObject>();
        public static Dictionary<string, GameObject> ballPrefabs = new Dictionary<string, GameObject>();


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
    }

}

