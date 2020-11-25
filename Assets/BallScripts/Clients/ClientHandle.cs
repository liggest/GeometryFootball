using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using BallScripts.Utils;
using BallScripts.GameLogics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace BallScripts.Clients
{
    public class ClientHandle //这里MonoBehavior有点意味不明，去掉了
    {
        public static void Welcome(Packet packet)
        {
            string msg = packet.ReadString();
            int id = packet.ReadInt();

            Debug.Log($"服务器消息：{msg}");
            Client.instance.myID = id;

            ClientSend.WelcomeReceived();

            Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        }

        public static void ClientConnectionRefused(Packet packet)
        {
            string msg = packet.ReadString();
            Debug.Log(msg);
            Client.instance.Disconnect();
            GameManager.instance.BeginLoadScene("TestScene"); //暂时先回主场景
        }

        public static void SceneLoadingStarted(Packet packet)
        {
            string sceneName = packet.ReadString();
            Actions.DisconnectedAction = ClientLogic.ClientDisconnection;
            ClientLogic.BeginSceneLoading(sceneName);
        }

        public static void StageObjectPositions(Packet packet)
        {
            while (packet.UnreadLength() >= 4)
            {
                StageObjectCategory category = (StageObjectCategory)packet.ReadInt();
                int id= packet.ReadInt();
                Vector3 position = packet.ReadVector3();
                StageManager.instance.GetStageObject(category, id)?.SetPosition(position);
            }
        }

        public static void StageObjectLocalPositions(Packet packet)
        {
            while (packet.UnreadLength() >= 4)
            {
                StageObjectCategory category = (StageObjectCategory)packet.ReadInt();
                int id = packet.ReadInt();
                Vector3 localPosition = packet.ReadVector3();
                StageManager.instance.GetStageObject(category, id)?.SetLocalPosition(localPosition);
            }
        }

        public static void StageObjectRotations(Packet packet)
        {
            while (packet.UnreadLength() >= 4)
            {
                StageObjectCategory category = (StageObjectCategory)packet.ReadInt();
                int id = packet.ReadInt();
                Quaternion rotation = packet.ReadQuaternion();
                StageManager.instance.GetStageObject(category, id)?.SetRotation(rotation);
            }
        }

        public static void StageObjectLocalRotations(Packet packet)
        {
            while (packet.UnreadLength() >= 4)
            {
                StageObjectCategory category = (StageObjectCategory)packet.ReadInt();
                int id = packet.ReadInt();
                Quaternion localRotation = packet.ReadQuaternion();
                StageManager.instance.GetStageObject(category, id)?.SetRotation(localRotation);
            }
        }

        public static void StageObjectLocalScales(Packet packet)
        {
            while (packet.UnreadLength() >= 4)
            {
                StageObjectCategory category = (StageObjectCategory)packet.ReadInt();
                int id = packet.ReadInt();
                Vector3 localScale = packet.ReadVector3();
                StageManager.instance.GetStageObject(category, id)?.SetLocalScale(localScale);
            }
        }

        public static void StageObjectSpawned(Packet packet)
        {
            BaseBuildInfo info = Deserialize<BaseBuildInfo>(packet);
            //Debug.Log(info.GetType());
            //Debug.Log(info is PlayerBuildInfo);
            //Debug.Log(info.GetType() == typeof(BaseBuildInfo));
            GameManager.instance.SpawnStageObject(info, BuildType.Client);
        }

        static T Deserialize<T>(Packet packet)
        {
            int length = packet.UnreadLength();
            byte[] objBytes = packet.ReadBytes(length);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.SurrogateSelector = SurrogateManager.GetSurrogateSelector();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(objBytes, 0, length);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                T obj = (T)formatter.Deserialize(stream);
                return obj;
            }
        }

        public static void StageObjectDespawned(Packet packet)
        {
            while (packet.UnreadLength() > 0)
            {
                StageObjectCategory category = (StageObjectCategory)packet.ReadInt();
                int id = packet.ReadInt();
                GameManager.instance.DespawnStageObject(category, id);
            }
        }
        public static void StageObjectRemoved(Packet packet)
        {
            while (packet.UnreadLength() > 0)
            {
                StageObjectCategory category = (StageObjectCategory)packet.ReadInt();
                int id = packet.ReadInt();
                StageManager.instance.RemoveStageObject(category, id);
            }
        }

        public static void StageObjectInfo(Packet packet)
        {
            StageObjectCategory category = (StageObjectCategory)packet.ReadInt();
            int id = packet.ReadInt();
            string route = packet.ReadString();
            object value = packet.ReadObject();
            NetworkMarkerManager.SetStageObjectInfo(category, id, route, value);
        }

        public static void StageObjectMethod(Packet packet)
        {
            StageObjectCategory category = (StageObjectCategory)packet.ReadInt();
            int id = packet.ReadInt();
            string route = packet.ReadString();
            ArrayList parameters = new ArrayList();
            while(packet.UnreadLength() > 0)
            {
                parameters.Add(packet.ReadObject());
            }
            NetworkMarkerManager.CallStageObjectMethod(category, id, route, true, parameters.ToArray());
        }

        public static void PlayerScored(Packet packet)
        {
            int playerID = packet.ReadInt();
            int playerScore = packet.ReadInt();
            int goalID = packet.ReadInt();
            int goalScore = packet.ReadInt();
            int value = packet.ReadInt();
            ClientLogic.PlayerScored(playerID, playerScore, goalID, goalScore, value);
        }

        public static void TeamLeft(Packet packet)
        {
            int playerID = packet.ReadInt();
            int playerScore = packet.ReadInt();
            TeamManager.instance.RemoveFromTeam(playerID, playerScore);
        }

        /*
        public static void PlayerSpawned(Packet packet)
        {
            int id = packet.ReadInt();
            string prefabName = packet.ReadString();
            ClientLogic.InitPlayer(id, prefabName);
        }
        */
        /*
        public static void SpawnPlayer(Packet packet)
        {
            int id = packet.ReadInt();
            string username = packet.ReadString();
            Vector3 position = packet.ReadVector3();
            Quaternion rotation = packet.ReadQuaternion();

            GameManager.instance.SpawnPlayers(id, username, position, rotation);
        }

        public static void PlayerPosition(Packet packet)
        {
            int id = packet.ReadInt();
            Vector3 position = packet.ReadVector3();

            if (GameManager.players.TryGetValue(id, out PlayerManager player))
            {
                player.transform.position = position;
            }
        }

        public static void PlayerRotation(Packet packet)
        {
            int id = packet.ReadInt();
            Quaternion rotation = packet.ReadQuaternion();

            if (GameManager.players.TryGetValue(id, out PlayerManager player))
            {
                player.transform.rotation = rotation;
            }
        }
        */
        public static void PlayerDisconnected(Packet packet)
        {
            int id = packet.ReadInt();
            //Destroy(GameManager.players[id].gameObject);
            //GameManager.players.Remove(id);
        }
        /*
        public static void PlayerHealth(Packet packet)
        {
            int id = packet.ReadInt();
            float health = packet.ReadFloat();
            GameManager.players[id].SetHealth(health);
        }

        public static void PlayerRespawned(Packet packet)
        {
            int id = packet.ReadInt();
            GameManager.players[id].Respawn();
        }

        public static void CreateItemSpawner(Packet packet)
        {
            int spawnerID = packet.ReadInt();
            Vector3 position = packet.ReadVector3();
            bool hasItem = packet.ReadBool();

            GameManager.instance.CreateItemSpawner(spawnerID, position, hasItem);
        }

        public static void ItemSpawned(Packet packet)
        {
            int spawnerID = packet.ReadInt();
            GameManager.itemSpawners[spawnerID].ItemSpawned();
        }

        public static void ItemPickedUp(Packet packet)
        {
            int spawnerID = packet.ReadInt();
            int playerID = packet.ReadInt();
            GameManager.itemSpawners[spawnerID].ItemPickedUp();
            GameManager.players[playerID].itemCount++;
        }

        public static void SpawnProjectile(Packet packet)
        {
            int projectileID = packet.ReadInt();
            Vector3 position = packet.ReadVector3();
            int playerID = packet.ReadInt();

            GameManager.instance.SpawnProjectile(projectileID, position);
            GameManager.players[playerID].itemCount--;
        }

        public static void ProjectilePosition(Packet packet)
        {
            int projectileID = packet.ReadInt();
            Vector3 position = packet.ReadVector3();

            if (GameManager.projectiles.TryGetValue(projectileID, out ProjectileManager projectile))
            {
                projectile.transform.position = position;
            }
        }

        public static void ProjectileExploded(Packet packet)
        {
            int projectileID = packet.ReadInt();
            Vector3 position = packet.ReadVector3();

            GameManager.projectiles[projectileID].Explode(position);
        }

        public static void SpawnEnemy(Packet packet)
        {
            int enemyID = packet.ReadInt();
            Vector3 position = packet.ReadVector3();

            GameManager.instance.SpawnEnemy(enemyID, position);
        }

        public static void EnemyPosition(Packet packet)
        {
            int enemyID = packet.ReadInt();
            Vector3 position = packet.ReadVector3();

            if (GameManager.enemies.TryGetValue(enemyID, out EnemyManager enemy))
            {
                enemy.transform.position = position;
            }
        }

        public static void EnemyHealth(Packet packet)
        {
            int enemyID = packet.ReadInt();
            float health = packet.ReadFloat();

            GameManager.enemies[enemyID].SetHealth(health);
        }
        */

        /*
        public static void UDPTest(Packet packet)
        {
            string msg = packet.ReadString();

            Debug.Log($"服务器UDP消息：{msg}");
            ClientSend.UDPTestReceived();
        }
        */
    }

}

