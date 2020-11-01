using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Servers;
using BallScripts.Clients;

namespace BallScripts.GameLogics
{
    public class PlayerBuilder : BaseBuilder<Player,PlayerBuildInfo>
    {

        public override Player BuildInClient(Player obj, PlayerBuildInfo info)
        {
            if (info.id == Clients.Client.instance.myID)
            {
                obj.gameObject.AddComponent<InputSender>();
            }
            return obj;
        }

        public override Player BuildInServer(Player obj, PlayerBuildInfo info)
        {
            InitPlayerRigidbody(obj.gameObject.AddComponent<Rigidbody>());
            obj.gameObject.AddComponent<InfoSender>();
            obj.gameObject.AddComponent<PlayerController>();
            /*
            obj.barList.ForEach((Bar bar) =>
            {
                bar.gameObject.AddComponent<BarCollision>();
                Debug.Log(bar.name);
            });*/
            return obj;
        }

        public override Player BuildObject(PlayerBuildInfo info)
        {
            Debug.Log(info.playerType);
            //if (ResourcesManager.playerPrefabs.TryGetValue(prefabName, out GameObject playerPrefab))
            GameObject playerPrefab = ResourcesManager.Get<GameObject>(info.prefabName);
            if (playerPrefab)
            {
                GameObject player = Object.Instantiate(playerPrefab);
                Player pl = player.GetComponent<Player>();
                pl.Init(info.category, info.id);
                return pl;
            }
            else
            {
                Debug.Log($"没有找到名为{info.prefabName}的Prefab");
            }
            return null;
        }

        public static Rigidbody InitPlayerRigidbody(Rigidbody rig)
        {
            rig.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rig.mass = 5;
            rig.angularDrag = 0.1f;
            rig.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            return rig;
        }

        public override bool CheckInfo(BaseBuildInfo info)
        {
            return info is PlayerBuildInfo;
        }
    }
}

