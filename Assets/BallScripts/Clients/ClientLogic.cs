using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;
using System;

namespace BallScripts.Clients {
    public class ClientLogic
    {
        public static void BeginSceneLoading(string sceneName)
        {
            GameManager.instance.BeginLoadScene(sceneName,(string s)=>
            {
                if (s == sceneName)
                {
                    ClientSend.SceneLoaded(s);
                }
            });
        }

        public static void InitPlayer(int clientID, string prefabName)
        {
            Player Extra(Player p){
                p.Init(StageObjectCategory.Player, clientID);
                if (clientID == Client.instance.myID)
                {
                    p.gameObject.AddComponent<InputSender>();
                }
                return p;
            };

            PlayerBuilder pb = new PlayerBuilder();
            pb.ExtraProcess += Extra;
            Player pl = pb.Build(prefabName, BuildType.Client);

            Debug.Log($"客户端{clientID} 初始化Player - {prefabName}");
        }

    }

}
