using System;

namespace BallScripts.Servers
{
    class Actions
    {
        public static Action<int> ClientTCPConnectedAction;
        public static Action<int> ClientUDPConnectedAction;
        public static Action<int> ClientDisconnectedAction;
        public static Action PlayerCountUpatedAction;
    }
}
