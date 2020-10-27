using UnityEngine;
using BallScripts.Utils;

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
                    GameLogics.GameManager.instance.BeginLoadScene("DemoStage",(string name)=> 
                    {
                        Debug.Log($"DemoStage加载完成！");
                        ThreadManager.ExecuteOnMainThread(()=> {
                            ServerLogic.AttachRigidbodyToAll(GameLogics.StageObjectCategory.Ball);
                        });
                        ThreadManager.ExecuteOnMainThread(ServerLogic.AttachInfoSendersToAll);
                    });
                    //暂且先载入DemoStage
                    Server.state = ServerState.InStage;
                    //Actions.ClientTCPConnectedAction -= LoadDemo;
                }
            }

            Actions.ClientTCPConnectedAction += LoadDemo;
            Actions.ClientUDPConnectedAction += (int cid) =>
            {
                ServerSend.SceneLoadingStarted(cid, "DemoStage");
            };
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
