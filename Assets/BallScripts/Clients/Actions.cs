using System;

namespace BallScripts.Clients
{
    class Actions
    {
#pragma warning disable 0649
        //#pragma去除警告
        public static Action TCPConnectedAction;
        public static Action UDPConnectedAction;
        public static Action TCPConnectTimeoutAction;
        public static Action DisconnectedAction;

#pragma warning restore 0649
    }
}
