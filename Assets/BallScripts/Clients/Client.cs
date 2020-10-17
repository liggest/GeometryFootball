using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Utils;
using System;
using System.Linq;
using System.Reflection;
using System.Net;
using System.Net.Sockets;


namespace BallScripts.Clients {
    public class Client : Singleton<Client>
    {
        public static int BufferSize = 4096;

        public string ip = "127.0.0.1";
        public int port = 6960;
        public TCP tcp;
        public UDP udp;
        private bool isConnecting = false;
        private bool isConnected = false;

        public int myID = 0;
        public string myName = "Anonymous";

        public delegate void PacketHandler(Packet packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        public void ConnectToServer()
        {
            tcp = new TCP();
            udp = new UDP();

            InitializeClientData();
            isConnecting = true;
            //isConnected = true;
            tcp.Connect();
        }

        private void InitializeClientData()
        {
            IEnumerable<ServerPackets> PacketMembers = Enum.GetValues(typeof(ServerPackets)) as IEnumerable<ServerPackets>; //得到ServerPackets的所有值
            IEnumerable<MethodInfo> HandleMethods = typeof(ClientHandle).GetMethods(BindingFlags.Public | BindingFlags.Static); //得到ClientHandle内所有public static方法
            Type PHType = typeof(PacketHandler); //PacketHandler委托的Type

            //PacketMembers.Select(x => x.ToString()).ToList().ForEach(x => Debug.Log(x));  //打印PacketMembers
            //HandleMethods.Select(x => x.Name).ToList().ForEach(x => Debug.Log(x));  //打印HandleMethods

            packetHandlers = PacketMembers.Join(HandleMethods, p => p.ToString(), h => h.Name, (p, h) => new { P = p, H = h }) //PacketMembers与HandleMethods连接，当p.ToString()==h.Name时，得到{ P = p, H = h }的列表
                .ToDictionary(k => (int)k.P, v => (PacketHandler)Delegate.CreateDelegate(PHType, v.H)); //将{ P = p, H = h }转化为<(int)ServerPackets,PacketHandler>的字典

            /*
            foreach (int key in packetHandlers.Keys)
            {
                Debug.Log($"{Enum.Parse(typeof(ServerPackets), key.ToString())}={key} : {packetHandlers[key].Method.Name}");
            }
            */ //打印packetHandlers

            //就可以不用下面的写法了
            /*
            packetHandlers = new Dictionary<int, PacketHandler>
            {
                { (int)ServerPackets.welcome,ClientHandle.Welcome },
                //{ (int)ServerPackets.spawnPlayer,ClientHandle.SpawnPlayer },
                //{ (int)ServerPackets.playerPosition,ClientHandle.PlayerPosition },
                //{ (int)ServerPackets.playerRotation,ClientHandle.PlayerRotation },
                { (int)ServerPackets.playerDisconnected,ClientHandle.PlayerDisconnected },
                //{ (int)ServerPackets.playerHealth,ClientHandle.PlayerHealth },
                //{ (int)ServerPackets.playerRespawned, ClientHandle.PlayerRespawned },
                //{ (int)ServerPackets.createItemSpawner,ClientHandle.CreateItemSpawner },
                //{ (int)ServerPackets.itemSpawned,ClientHandle.ItemSpawned },
                //{ (int)ServerPackets.itemPickedUp,ClientHandle.ItemPickedUp },
                //{ (int)ServerPackets.spawnProjectile,ClientHandle.SpawnProjectile },
                //{ (int)ServerPackets.projectilePosition,ClientHandle.ProjectilePosition },
                //{ (int)ServerPackets.projectileExploded,ClientHandle.ProjectileExploded },
                //{ (int)ServerPackets.spawnEnemy,ClientHandle.SpawnEnemy },
                //{ (int)ServerPackets.enemyPosition,ClientHandle.EnemyPosition },
                //{ (int)ServerPackets.enemyHealth,ClientHandle.EnemyHealth }
                //{ (int)ServerPackets.UDPTest,ClientHandle.UDPTest }
            };
            */
            Debug.Log("初始化客户端数据和 Packets");

        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        private void Disconnect()
        {
            if (isConnecting || isConnected)
            {
                isConnecting = false;
                isConnected = false;
                tcp.socket?.Close();
                udp.socket?.Close();

                Debug.Log("已同服务器断开连接");
                Actions.DisconnectedAction?.Invoke();
            }
        }

        IEnumerator TCPConnectTimeout()
        {
            for (int i = 0; i < 50; i++)
            {
                if (isConnected || !isConnecting)
                {
                    yield break;
                }
                yield return new WaitForSeconds(0.1f);
            }
            if (isConnected || !isConnecting)
            {
                yield break;
            }
            else
            {
                Disconnect();
                Debug.Log("TCP连接超时");
                Actions.TCPConnectTimeoutAction?.Invoke();
            }
        }

        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private byte[] receiveBuffer;
            private Packet receiveData;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = BufferSize,
                    SendBufferSize = BufferSize
                };

                receiveBuffer = new byte[BufferSize];
                socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
                instance.StartCoroutine(instance.TCPConnectTimeout());
            }

            private void ConnectCallback(IAsyncResult result)
            {
                Debug.Log("结束握手");

                socket.EndConnect(result);
                Debug.Log("结束握手!!!");
                instance.isConnecting = false;
                if (!socket.Connected)
                {
                    Debug.Log("与服务器连接失败！");
                    return;
                }
                instance.isConnected = true;
                stream = socket.GetStream();
                receiveData = new Packet();

                Actions.TCPConnectedAction?.Invoke();

                stream.BeginRead(receiveBuffer, 0, BufferSize, ReceiveCallback, null);
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int length = stream.EndRead(result);
                    if (length <= 0)
                    {
                        instance.Disconnect();
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
                    Console.WriteLine($"接收TCP数据时出错：{ex}");
                    Disconnect();
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
                    Console.WriteLine($"向服务器发送TCP消息时出错：{ex}");
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
                            packetHandlers[packetID](packet);
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
                instance.Disconnect();
                stream = null;
                receiveBuffer = null;
                receiveData = null;
                socket = null;

            }
        }

        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
            }

            public void Connect(int localPort)
            {
                socket = new UdpClient(localPort);
                socket.Connect(endPoint);

                Actions.UDPConnectedAction?.Invoke();

                socket.BeginReceive(ReceiveCallback, null);

                using (Packet packet = new Packet())//用于初始化与服务端的连接，开启local port
                {
                    SendData(packet);
                }
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    byte[] data = socket.EndReceive(result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);

                    if (data.Length < 4) //不足一个整数，代表没有数据中没有Packet
                    {
                        instance.Disconnect();
                        return;
                    }

                    HandleData(data);
                }
                catch
                {
                    Disconnect();
                }
            }

            public void SendData(Packet packet)
            {
                try
                {
                    packet.InsertInt(instance.myID);//这样服务端才能分清Packet是谁发的
                                                    //服务端似乎会用一个UDPClient处理所有发来的UPD报文
                    if (socket != null)
                    {
                        socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"向服务器发送UDP消息时出错：{ex}");
                }
            }

            private void HandleData(byte[] data)
            {
                using (Packet packet = new Packet(data))
                {
                    int packetlen = packet.ReadInt();
                    data = packet.ReadBytes(packetlen);
                }

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(data))
                    {
                        int packetID = packet.ReadInt();
                        packetHandlers[packetID](packet);
                    }
                });
            }

            public void Disconnect()
            {
                instance.Disconnect();
                endPoint = null;
                socket = null;
            }
        }

    }

}

