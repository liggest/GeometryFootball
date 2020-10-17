using UnityEngine;
using BallScripts.Utils;

namespace BallScripts.Servers
{
    public class NetworkManager : Singleton<NetworkManager>
    {
        //public static NetworkManager instance;

        //public GameObject playerPrefab;
        //public GameObject projectilePrefab;
        //public GameObject enemyPrefab;

        /*
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("单例已经存在，当前对象会被销毁");
                Destroy(this);
            }
        }
        */

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
