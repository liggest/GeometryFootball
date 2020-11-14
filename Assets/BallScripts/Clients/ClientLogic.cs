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

        public static void GoalScored(int goalID, int goalScore, int value)
        {
            Goal goal = StageManager.instance.GetStageObject(StageObjectCategory.Goal, goalID) as Goal;
            if (goal)
            {
                //与服务器进球前的球门分数同步
                int temp = goalScore - value;
                if (goal.HasTeam)
                {
                    TeamManager.instance.teams[goal.TeamID].Score += temp - goal.Score;
                }
                goal.Score = temp;
                goal.TryScore(value);//进球得分
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
