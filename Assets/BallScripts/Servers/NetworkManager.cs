using UnityEngine;
using BallScripts.Utils;
using BallScripts.GameLogics;

namespace BallScripts.Servers
{
    public class NetworkManager : Singleton<NetworkManager>
    {

        //public GameObject playerPrefab;
        //public GameObject projectilePrefab;
        //public GameObject enemyPrefab;


        private void Start()
        {
            //System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            QualitySettings.vSyncCount = 0; //关掉vSync
            Application.targetFrameRate = 60;
            //因为服务端没有图形界面，能够运行的很快，不加限制的话一秒能跑很多帧，造成较高CPU占用
        }

        public void StartServer(int port = 6960)
        {
            Server.Start(50, port);

            //下面是临时处理措施
            void LoadDemo(int cid)
            {
                if (Server.state == ServerState.Started && Server.PlayerCount == 1 && cid > 0) 
                {
                    Debug.Log($"服务端尝试加载DemoStage...");
                    GameManager.instance.RefreshBeforeLoad();
                    GameManager.instance.BeginLoadScene("DemoStage",(string name)=> 
                    {
                        Debug.Log($"DemoStage加载完成！");
                        //ThreadManager.ExecuteOnMainThread(()=> {
                        //    ServerLogic.AttachRigidbodyToAll(StageObjectCategory.Ball);
                        //});
                        ThreadManager.ExecuteOnMainThread(ServerLogic.AttachInfoSendersToAll);
                        ThreadManager.ExecuteOnMainThread(() =>
                        {
                            ServerLogic.InitBall("DemoBall");
                        });
                    });
                    //暂且先载入DemoStage
                    Server.state = ServerState.InStage;
                    Actions.PlayerCountUpatedAction = NoPlayer;
                    //Actions.ClientTCPConnectedAction -= LoadDemo;
                }
            }

            void NoPlayer(int playerNum)
            {
                if (Server.PlayerCount == 0)
                {
                    GameManager.instance.BeginLoadScene("TestScene", (string name) =>
                    {
                        Debug.Log($"TestStage加载完成！");
                    });
                    Server.state = ServerState.Started;
                }
            }

            Actions.ClientTCPConnectedAction += LoadDemo;
            Actions.ClientUDPConnectedAction += (int cid) =>
            {
                if (!string.IsNullOrEmpty(Server.clients[cid].username))
                {
                    ServerSend.SceneLoadingStarted(cid, "DemoStage");
                }
            };
            Actions.ClientDisconnectedAction += ServerLogic.PlayerDisconnected;
        }

        private void OnApplicationQuit()
        {
            Server.Stop();
        }


        /*
        public Player InstantiatePlayer()
        {
            return Instantiate(playerPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity).GetComponent<Player>();
        }

        public Projectile InstantiateProjectile(Transform shootStart)
        {
            return Instantiate(projectilePrefab, shootStart.position + shootStart.forward * 0.7f, Quaternion.identity).GetComponent<Projectile>();
        }

        public void InstantiateEnemy(Vector3 position)
        {
            Instantiate(enemyPrefab, position, Quaternion.identity);
        }
        */
    }

}
