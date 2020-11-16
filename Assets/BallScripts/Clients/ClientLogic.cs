using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;
using BallScripts.Utils;
using System;

namespace BallScripts.Clients {
    public class ClientLogic
    {
        public static void BeginSceneLoading(string sceneName)
        {
            RefreshBeforeLoad();
            GameManager.instance.RefreshBeforeLoad();
            GameManager.instance.BeginLoadScene(sceneName,(string s)=>
            {
                if (s == sceneName)
                {
                    ClientSend.SceneLoaded(s);
                    ThreadManager.ExecuteOnMainThread(SendStageSituation);
                }
            });
        }

        public static void RefreshBeforeLoad()
        {
            NetworkMarkerManager.objCache.Clear();
        }

        public static void SendStageSituation()
        {
            ClientSend.StageSituation(StageManager.instance.GetStageObjectPairs());
        }

        public static void PlayerScored(int playerID, int playerScore, int goalID, int goalScore, int value)
        {
            Player player = StageManager.instance.GetStageObject(StageObjectCategory.Player, playerID) as Player;
            if (player)
            {
                //与服务器那边，进球前的玩家分数同步
                int temp = playerScore - value;
                player.team.Score += temp - player.Score;
                player.Score = temp;
                player.AddSocre(value);//进球得分
            }
            Goal goal = StageManager.instance.GetStageObject(StageObjectCategory.Goal, goalID) as Goal;
            if (goal)
            {
                int temp = goalScore - value;
                goal.Score = temp;
                goal.AddSocre(value);//进球得分
            }
        }

        public static void ClientDisconnection()
        {
            ThreadManager.ExecuteOnMainThread(() => { GameManager.instance.BeginLoadScene("TestScene"); });//暂时先回主场景
            Actions.DisconnectedAction -= ClientDisconnection;
        }

        /*
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
        */
    }

}
