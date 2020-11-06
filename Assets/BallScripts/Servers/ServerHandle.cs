using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BallScripts.Utils;
using BallScripts.GameLogics;

namespace BallScripts.Servers
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int clientID, Packet packet)
        {
            int idcheck = packet.ReadInt();
            string username = packet.ReadString();

            Debug.Log($"{Server.clients[clientID].tcp.socket.Client.RemoteEndPoint} 连接成功，成为玩家{clientID}");
            if (clientID != idcheck)
            {
                Debug.Log($"玩家ID检查出错！玩家{clientID}-{username}拥有了错误的客户端ID：{idcheck}");
            }
            else
            {
                Server.clients[clientID].username = username;
            }
            //Server.clients[clientID].SendIntoGame(username);
        }

        public static void SceneLoaded(int clientID, Packet packet)
        {
            string sceneName = packet.ReadString();
            string prefabName = packet.ReadString(); //到时候删
            Debug.Log($"客户端{clientID}加载好了场地{sceneName}");

            ServerLogic.InitClientPlayer(clientID, prefabName);
            //ServerSend.PlayerSpawned(clientID, prefabName);
        }

        public static void InputPacket(int clientID,Packet packet)
        {
            while (packet.UnreadLength() > 0)
            {
                InputType key = (InputType)packet.ReadInt();
                float value = 0;
                if (key != InputType.ultimate)
                {
                    value = packet.ReadFloat();
                }
                else
                {
                    value = 1;
                }
                PlayerController.SetClientBuffer(clientID, key, value);
            }
        }

        public static void StageSituation(int clientID,Packet packet)
        {
            List<StageObjectPair> clientObjs = new List<StageObjectPair>();
            while (packet.UnreadLength() > 0)
            {
                clientObjs.Add(new StageObjectPair { category = (StageObjectCategory)packet.ReadInt(), id = packet.ReadInt() });
            }
            ServerLogic.StageSynchronize(clientID, clientObjs);
        }

        /*
        public static void PlayerMovement(int clientID, Packet packet)
        {
            bool[] inputs = new bool[packet.ReadInt()];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = packet.ReadBool();
            }
            Quaternion rotation = packet.ReadQuaternion();

            Server.clients[clientID].player.SetInput(inputs, rotation);

        }

        public static void PlayerShoot(int clientID, Packet packet)
        {
            Vector3 facing = packet.ReadVector3();

            Server.clients[clientID].player.Shoot(facing);
        }

        public static void PlayerUseItem(int clientID,Packet packet)
        {
            Vector3 facing = packet.ReadVector3();
            Server.clients[clientID].player.ThrowItem(facing);
        }
        */

        /*
        public static void UDPTestReceived(int clientID,Packet packet)
        {
            string msg = packet.ReadString();
            Debug.Log($"收到UDP消息：{msg}");
        }
        */
    }

}
