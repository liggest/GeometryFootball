using System;

namespace BallScripts.Servers
{
    class Actions
    {
#pragma warning disable 0649
        //#pragma去除警告

        /// <summary>
        /// 服务器启动后调用
        /// </summary>
        public static Action ServerStartedAction;
        /// <summary>
        /// 客户端TCP连接成功后调用，int参数为连接的客户端的id
        /// </summary>
        public static Action<int> ClientTCPConnectedAction;
        /// <summary>
        /// 客户端UDP连接成功后调用（晚于TCP连接），int参数为连接的客户端的id
        /// </summary>
        public static Action<int> ClientUDPConnectedAction;
        /// <summary>
        /// 客户端断开连接（TCP&UDP）后调用，int参数为断开的客户端的id
        /// </summary>
        public static Action<int> ClientDisconnectedAction;
        /// <summary>
        /// 服务器连接的客户端数变动后调用（先于各种连接、断开），int参数为连接客户端变动的数量
        /// </summary>
        public static Action<int> PlayerCountUpatedAction;

#pragma warning restore 0649
    }
}
