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
        private void Start()
        {
            ResourcesManager.LoadAll();
            InitBuilderChain();
        }

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


        GenericBuilder rootBuilder;

        public void InitBuilderChain()
        {
            rootBuilder = new GenericBuilder();
            PlayerBuilder pb = new PlayerBuilder();
            rootBuilder.Next = pb;
        }

        public BaseStageObject SpawnStageObject(BaseBuildInfo info,BuildType type)
        {
            if (type == BuildType.Client)
            {
                Debug.Log($"客户端{Clients.Client.instance.myID} 生成了 {info.prefabName}");
            }
            else
            {
                Debug.Log($"服务器 生成了 {info.prefabName}");
            }
            return rootBuilder.GetCorrectBuilder(info).Build(info, type);
        }

    }

}