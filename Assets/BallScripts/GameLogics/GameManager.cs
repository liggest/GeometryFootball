using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using BallScripts.Utils;

namespace BallScripts.GameLogics
{
    public class GameManager : Singleton<GameManager>
    {
        public string CurrentStageName { get => SceneManager.GetActiveScene().name; }

        public void BeginLoadScene(string sceneName, Action<string> SceneLoadedAction = null)
        {
            StartCoroutine(SceneLoading(sceneName, SceneLoadedAction));
        }

        IEnumerator SceneLoading(string sceneName, Action<string> SceneLoadedAction)
        {
            AsyncOperation ap = SceneManager.LoadSceneAsync(sceneName);
            ap.allowSceneActivation = true;
            while (ap.progress < 1.0f)
            {
                yield return new WaitForEndOfFrame();
            }
            SceneLoadedAction?.Invoke(sceneName);
            Debug.Log("加载完了");
        }

        public void BeServer(int port)
        {
            Servers.NetworkManager.instance.StartServer(port);
            ThreadManager.ExecuteOnMainThread(Servers.ServerLogic.InitInfoBuffer);
        }

        public void BeClient(string ip, int port, string name)
        {
            Clients.Client.instance.ip = ip;
            Clients.Client.instance.port = port;
            Clients.Client.instance.myName = name;
            Clients.Client.instance.ConnectToServer();
        }

    }

}