using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using BallScripts.Utils;

namespace BallScripts.Servers
{

    public class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static int PlayerCount { get; private set; }

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public static Dictionary<string, int> onlineClients = new Dictionary<string, int>();
        public delegate void PacketHandler(int clientID, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static ServerState state = ServerState.Offline;

        public static void Start(int maxplayers, int port)
        {
            MaxPlayers = maxplayers;
            Port = port;

            Debug.Log($"正在启动服务端……");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            state = ServerState.Started;
            ThreadManager.ExecuteOnMainThread(() => Actions.ServerStartedAction?.Invoke());
            Debug.Log($"服务端在{Port}上开始运行~");

        }

        private static void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Debug.Log($"收到来自 {client.Client.RemoteEndPoint} 的连接……");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(client);
                    return;
                }
            }

            Debug.Log($"{client.Client.RemoteEndPoint} 连接失败！ *服务器已满*");
        }

        private static void UDPReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                if (udpListener == null)
                {
                    return;
                }
                byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4)
                {
                    return;
                }

                using (Packet packet = new Packet(data))
                {
                    int clientID = packet.ReadInt();
                    if (clientID == 0) //不应该发生
                    {
                        return;
                    }

                    //如果是，则是初次连接
                    //发来的包是为了让服务器记录客户端用的
                    if (clients[clientID].udp.endPoint == null)
                    {
                        clients[clientID].udp.Connect(clientEndPoint);
                        return;
                    }

                    if (clients[clientID].udp.endPoint.ToString() == clientEndPoint.ToString())
                    {
                        clients[clientID].udp.HandleData(packet);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"接收UDP数据时出错：{ex}");
                //执行Stop()后，上述逻辑还在继续
                //因为udpListener已经Close了，导致执行到udpListener.EndReceive处产生错误
                //所以会产生一个上述输出
                //在Stop中将udpListener置空，能让这个输出消失
            }
        }

        public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
        {
            try
            {
                if (clientEndPoint != null)
                {
                    udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"向{clientEndPoint}发送UDP数据时出错：{ex}");
            }
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            PlayerCount = 0;

            IEnumerable<ClientPackets> PacketMembers = Enum.GetValues(typeof(ClientPackets)) as IEnumerable<ClientPackets>; //得到ClientPackets的所有值
            IEnumerable<MethodInfo> HandleMethods = typeof(ServerHandle).GetMethods(BindingFlags.Public | BindingFlags.Static); //得到ServerHandle内所有public static方法
            Type PHType = typeof(PacketHandler); //PacketHandler委托的Type

            //PacketMembers.Select(x => x.ToString()).ToList().ForEach(x => Debug.Log(x));  //打印PacketMembers
            //HandleMethods.Select(x => x.Name).ToList().ForEach(x => Debug.Log(x));  //打印HandleMethods

            packetHandlers = PacketMembers.Join(HandleMethods, p => p.ToString(), h => h.Name, (p, h) => new { P = p, H = h }) //PacketMembers与HandleMethods连接，当p.ToString()==h.Name时，得到{ P = p, H = h }的列表
                .ToDictionary(k => (int)k.P, v => (PacketHandler)Delegate.CreateDelegate(PHType,v.H)); //将{ P = p, H = h }转化为<(int)ClientPackets,PacketHandler>的字典

            /*
            foreach (int key in packetHandlers.Keys)
            {
                Debug.Log($"{Enum.Parse(typeof(ClientPackets), key.ToString())}={key} : {packetHandlers[key].Method.Name}");
            }
            */ //打印packetHandlers

            //就可以不用下面的写法了
            /*
            packetHandlers = new Dictionary<int, PacketHandler>
            {
                { (int)ClientPackets.WelcomeReceived,ServerHandle.WelcomeReceived },
                //{ (int)ClientPackets.playerMovement,ServerHandle.PlayerMovement },
                //{ (int)ClientPackets.playerShoot,ServerHandle.PlayerShoot },
                //{ (int)ClientPackets.playerUseItem,ServerHandle.PlayerUseItem }
                //{ (int)ClientPackets.UDPTestReceived,ServerHandle.UDPTestReceived }
            };
            */

            Debug.Log("初始化服务端数据和 Packets");
        }

        public static void Stop()
        {
            tcpListener.Stop();
            udpListener.Close();
            udpListener = null;
            clients.Clear();
            state = ServerState.Offline;
        }

        public static void UpdatePlayerCount(int value)
        {
            PlayerCount += value;
            ThreadManager.ExecuteOnMainThread(() => Actions.PlayerCountUpatedAction?.Invoke(value));
        }
    }

    public enum ServerState
    {
        Offline,
        Started,
        InStage
    }

}