﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using BallScripts.Utils;

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

