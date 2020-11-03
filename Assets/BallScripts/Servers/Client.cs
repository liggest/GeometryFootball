using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using BallScripts.Utils;

namespace BallScripts.Servers
{
    public class Client
    {
        public static int BufferSize = 4096;

        public int id;

        //public Player player;

        public TCP tcp;
        public UDP udp;

        public Client(int _id)
        {
            id = _id;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private byte[] receiveBuffer;
            private Packet receiveData;
            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = BufferSize;
                socket.SendBufferSize = BufferSize;

                stream = socket.GetStream();
                receiveData = new Packet();
                receiveBuffer = new byte[BufferSize];

                stream.BeginRead(receiveBuffer, 0, BufferSize, ReceiveCallback, null);

                ServerSend.Welcome(id, "来自服务器的欢迎~");
                Server.UpdatePlayerCount(1);
                ThreadManager.ExecuteOnMainThread(() => Actions.ClientTCPConnectedAction?.Invoke(id));
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int length = stream.EndRead(result);
                    if (length <= 0)
                    {
                        Server.clients[id].Disconnect();
                        return;
                    }
                    byte[] data = new byte[length];
                    Array.Copy(receiveBuffer, data, length);

                    receiveData.Reset(HandleData(data)); //传输中可能会出现只发来部分packet的情况
                                                         //通过HandleData确定receiveData（Packet）是否要重置

                    stream.BeginRead(receiveBuffer, 0, BufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    Debug.Log($"接收TCP数据时出错：{ex}");
                    Server.clients[id].Disconnect();
                }
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log($"向玩家 {id} 发送TCP消息时出错：{ex}");
                }
            }

            private bool HandleData(byte[] data)
            {
                int packetlen = 0;

                receiveData.SetBytes(data);
                //判断Packet头是否有一个int，因为约定，这个int肯定是WriteLength写下的值
                if (receiveData.UnreadLength() >= 4)
                {
                    packetlen = receiveData.ReadInt();
                    if (packetlen <= 0)
                    {
                        return true;
                    }
                }

                while (packetlen > 0 && packetlen <= receiveData.UnreadLength())
                {
                    byte[] packetBytes = receiveData.ReadBytes(packetlen);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet packet = new Packet(packetBytes))
                        {
                            int packetID = packet.ReadInt();
                            Server.packetHandlers[packetID](id, packet);
                        }
                    });

                    packetlen = 0;
                    if (receiveData.UnreadLength() >= 4)
                    {
                        packetlen = receiveData.ReadInt();
                        if (packetlen <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (packetlen <= 1)
                {
                    return true;
                }
                return false;

            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receiveData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP
        {
            public IPEndPoint endPoint;

            private int id;

            public UDP(int _id)
            {
                id = _id;
            }

            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
                //ServerSend.UDPTest(id);
                ThreadManager.ExecuteOnMainThread(() => Actions.ClientUDPConnectedAction?.Invoke(id));
            }

            public void SendData(Packet packet)
            {
                Server.SendUDPData(endPoint, packet);
            }

            public void HandleData(Packet packetData)
            {
                int packetlen = packetData.ReadInt();
                byte[] data = packetData.ReadBytes(packetlen);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(data))
                    {
                        int packetID = packet.ReadInt();
                        Server.packetHandlers[packetID](id, packet);
                    }
                });
            }

            public void Disconnect()
            {
                endPoint = null;
            }
        }
        /*
        public void SendIntoGame(string playerName)
        {
            //player = new Player(id, playerName, new Vector3(0, 0, 0));
            player = NetworkManager.instance.InstantiatePlayer();
            player.Initialize(id, playerName);

            foreach (Client client in Server.clients.Values)
            {
                if (client.player != null)
                {
                    if (client.id != id)
                    {
                        ServerSend.SpawnPlayer(id, client.player);
                    }
                }
            }

            foreach (Client client in Server.clients.Values)
            {
                if (client.player != null)
                {
                    ServerSend.SpawnPlayer(client.id, player);
                }
            }

            foreach (ItemSpawner itemSpawner in ItemSpawner.spawners.Values)
            {
                ServerSend.CreateItemSpawner(id, itemSpawner.spawnerID, itemSpawner.transform.position, itemSpawner.hasItem);
            }

            foreach (Enemy enemy in Enemy.enemies.Values)
            {
                ServerSend.SpawnEnemy(id, enemy);
            }
        }
        */

        private void Disconnect()
        {
            Debug.Log($"{tcp.socket.Client.RemoteEndPoint} 断开了连接");
            //ThreadManager.ExecuteOnMainThread(() =>
            //{
            //    UnityEngine.Object.Destroy(player.gameObject);
            //    player = null;
            //});
            tcp.Disconnect();
            udp.Disconnect();
            Server.UpdatePlayerCount(-1);

            ThreadManager.ExecuteOnMainThread(() => Actions.ClientDisconnectedAction?.Invoke(id));

            //ServerSend.PlayerDisconnected(id);
        }
    }

}

