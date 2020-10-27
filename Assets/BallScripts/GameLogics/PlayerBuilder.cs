using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Servers;

namespace BallScripts.GameLogics
{
    public class PlayerBuilder : BaseBuilder<Player>
    {
        public override Player BuildInClient(Player obj)
        {
            return obj;
        }

        public override Player BuildInServer(Player obj)
        {
            InitPlayerRigidbody(obj.gameObject.AddComponent<Rigidbody>());
            obj.gameObject.AddComponent<InfoSender>();
            obj.gameObject.AddComponent<PlayerController>();
            return obj;
        }

        public override Player BuildObject(string prefabName)
        {
            if (ResourcesManager.playerPrefabs.TryGetValue(prefabName, out GameObject playerPrefab))
            {
                GameObject player = Object.Instantiate(playerPrefab);
                Player pl = player.GetComponent<Player>();
                return pl;
            }
            else
            {
                Debug.Log("没有找到指定名字的PlayerPrefab");
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
    }
}

