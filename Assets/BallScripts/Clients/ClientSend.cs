﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Utils;
using BallScripts.GameLogics;

namespace BallScripts.Clients
{
    public class ClientSend //这里MonoBehavior有点意味不明，去掉了
    {
        #region Packet
        public static void WelcomeReceived()
        {
            using (Packet packet = new Packet((int)ClientPackets.WelcomeReceived))
            {
                packet.Write(Client.instance.myID);
                packet.Write(Client.instance.myName);

                SendTCPData(packet);
            }
        }

        public static void SceneLoaded(string sceneName)
        {
            using (Packet packet = new Packet((int)ClientPackets.SceneLoaded))
            {
                packet.Write(sceneName);
                packet.Write("DemoPlayer"); //这个回头要删掉
                SendTCPData(packet);
            }
        }

        public static void InputPacket(List<InputHolder> buffer)
        {
            using (Packet packet = new Packet((int)ClientPackets.InputPacket))
            {
                foreach (InputHolder holder in buffer)
                {
                    packet.Write((int)holder.key);
                    if (holder.key != InputType.ultimate)
                    {
                        packet.Write(holder.value);
                    }
                }
                SendUDPData(packet);
            }
        }

        public static void StageSituation(List<StageObjectPair> objs)
        {
            using (Packet packet = new Packet((int)ClientPackets.StageSituation))
            {
                for (int i = 0; i < objs.Count; i++)
                {
                    StageObjectPair pair = objs[i];
                    packet.Write((int)pair.category);
                    packet.Write(pair.id);
                }
                SendTCPData(packet);
            }
        }

        /*
        public static void PlayerMovement(bool[] inputs)
        {
            using (Packet packet = new Packet((int)ClientPackets.playerMovement))
            {
                packet.Write(inputs.Length);
                foreach (bool inputbool in inputs)
                {
                    packet.Write(inputbool);
                }
                packet.Write(GameManager.players[Client.instance.myID].transform.rotation);

                SendUDPData(packet);
            }
        }

        public static void PlayerShoot(Vector3 facing)
        {
            using (Packet packet = new Packet((int)ClientPackets.playerShoot))
            {
                packet.Write(facing);
                SendTCPData(packet);
            }
        }

        public static void PlayerUseItem(Vector3 facing)
        {
            using (Packet packet = new Packet((int)ClientPackets.playerUseItem))
            {
                packet.Write(facing);
                SendTCPData(packet);
            }
        }
        */

        /*
        public static void UDPTestReceived()
        {
            using (Packet packet = new Packet((int)ClientPackets.UDPTestReceived))
            {
                packet.Write($"玩家{Client.instance.myID}收到了UDP测试包");
                SendUDPData(packet);
            }
        }
        */
        #endregion

        #region SendData
        private static void SendTCPData(Packet packet)
        {
            packet.WriteLength();
            Client.instance.tcp.SendData(packet);
        }

        private static void SendUDPData(Packet packet)
        {
            packet.WriteLength();
            Client.instance.udp.SendData(packet);
        }
        #endregion
    }
}

