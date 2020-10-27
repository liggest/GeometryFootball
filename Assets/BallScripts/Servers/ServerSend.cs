using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Utils;

namespace BallScripts.Servers
{
    public class ServerSend
    {

        #region Packet
        public static void Welcome(int clientID, string msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.Welcome))
            {
                packet.Write(msg);
                packet.Write(clientID);

                SendTCPData(clientID, packet);
            }
        }

        public static void SceneLoadingStarted(int clientID,string sceneName)
        {
            using (Packet packet = new Packet((int)ServerPackets.SceneLoadingStarted))
            {
                packet.Write(sceneName);

                SendTCPData(clientID, packet);
            }
        }

        public static void StageObjectPosition(List<Vector3Holder> posList)
        {
            using (Packet packet = new Packet((int)ServerPackets.StageObjectPosition))
            {
                foreach (Vector3Holder holder in posList)
                {
                    packet.Write((int)holder.category);
                    packet.Write(holder.id);
                    packet.Write(holder.vect);
                }
                SendUDPDataToAll(packet);
            }
        }

        public static void StageObjectRotation(List<QuaternionHolder> rotList)
        {
            using (Packet packet = new Packet((int)ServerPackets.StageObjectRotation))
            {
                foreach (QuaternionHolder holder in rotList)
                {
                    packet.Write((int)holder.category);
                    packet.Write(holder.id);
                    packet.Write(holder.quat);
                }
                SendUDPDataToAll(packet);
            }
        }

        /*
        public static void SpawnPlayer(int clientID, Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                packet.Write(player.id);
                packet.Write(player.username);
                packet.Write(player.transform.position);
                packet.Write(player.transform.rotation);

                SendTCPData(clientID, packet);
            }
        }

        public static void PlayerPosition(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerPosition))
            {
                packet.Write(player.id);
                packet.Write(player.transform.position);

                SendUDPDataToAll(packet);
            }
        }

        public static void PlayerRotation(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerRotation))
            {
                packet.Write(player.id);
                packet.Write(player.transform.rotation);

                SendUDPDataToAll(player.id, packet);
                //不发给player所在的客户端，服务器并不对rotation做出修改
                //发的话可能会产生抖动之类的，需要更高级的技巧来同步
            }
        }
        */
        public static void PlayerDisconnected(int playerID)
        {
            using (Packet packet = new Packet((int)ServerPackets.PlayerDisconnected))
            {
                packet.Write(playerID);
                SendTCPDataToAll(packet);
            }
        }
        /*
        public static void PlayerHealth(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerHealth)) 
            {
                packet.Write(player.id);
                packet.Write(player.health);

                SendTCPDataToAll(packet);
            }
        }

        public static void PlayerRespawned(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerRespawned)) 
            {
                packet.Write(player.id);

                SendTCPDataToAll(packet);
            }
        }

        public static void CreateItemSpawner(int clientID,int spawnerID,Vector3 spawnerPos,bool hasItem)
        {
            using (Packet packet = new Packet((int)ServerPackets.createItemSpawner)) 
            {
                packet.Write(spawnerID);
                packet.Write(spawnerPos);
                packet.Write(hasItem);

                SendTCPData(clientID, packet);
            }
        }

        public static void ItemSpawned(int spawnerID)
        {
            using (Packet packet = new Packet((int)ServerPackets.itemSpawned))
            {
                packet.Write(spawnerID);
                SendTCPDataToAll(packet);
            }
        }

        public static void ItemPickedUp(int spawnerID,int playerID)
        {
            using (Packet packet = new Packet((int)ServerPackets.itemPickedUp))
            {
                packet.Write(spawnerID);
                packet.Write(playerID);
                SendTCPDataToAll(packet);
            }
        }

        public static void SpawnProjectile(Projectile projectile,int playerID)
        {
            using (Packet packet = new Packet((int)ServerPackets.spawnProjectile))
            {
                packet.Write(projectile.id);
                packet.Write(projectile.transform.position);
                packet.Write(playerID);

                SendTCPDataToAll(packet);
            }
        }

        public static void ProjectilePosition(Projectile projectile)
        {
            using (Packet packet = new Packet((int)ServerPackets.projectilePosition))
            {
                packet.Write(projectile.id);
                packet.Write(projectile.transform.position);

                SendUDPDataToAll(packet);
            }
        }

        public static void ProjectileExploded(Projectile projectile)
        {
            using (Packet packet = new Packet((int)ServerPackets.projectileExploded))
            {
                packet.Write(projectile.id);
                packet.Write(projectile.transform.position);

                SendTCPDataToAll(packet);
            }
        }

        public static void SpawnEnemy(Enemy enenmy)
        {
            using (Packet packet = new Packet((int)ServerPackets.spawnEnemy))
            {
                SendTCPDataToAll(SpawnEnemyData(enenmy, packet));
            }
        }

        public static void SpawnEnemy(int clientid, Enemy enenmy)
        {
            using (Packet packet = new Packet((int)ServerPackets.spawnEnemy))
            {
                SendTCPData(clientid, SpawnEnemyData(enenmy, packet));
            }
        }

        private static Packet SpawnEnemyData(Enemy enemy,Packet packet)
        {
            packet.Write(enemy.id);
            packet.Write(enemy.transform.position);
            return packet;
        }

        public static void EnemyPosition(Enemy enemy)
        {
            using (Packet packet = new Packet((int)ServerPackets.enemyPosition))
            {
                packet.Write(enemy.id);
                packet.Write(enemy.transform.position);

                SendUDPDataToAll(packet);
            }
        }

        public static void EnemyHealth(Enemy enemy)
        {
            using (Packet packet = new Packet((int)ServerPackets.enemyHealth))
            {
                packet.Write(enemy.id);
                packet.Write(enemy.health);

                SendTCPDataToAll(packet);
            }
        }
        */

        /*
        public static void UDPTest(int clientID)
        {
            using (Packet packet = new Packet((int)ServerPackets.UDPTest))
            {
                packet.Write("这是一个UDP测试");

                SendUDPData(clientID, packet);
            }
        }
        */
        #endregion

        #region SendData
        private static void SendTCPData(int clientID, Packet packet)
        {
            packet.WriteLength();
            Server.clients[clientID].tcp.SendData(packet);
        }

        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(packet);
            }
        }

        private static void SendTCPDataToAll(int expectClientID, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != expectClientID)
                {
                    Server.clients[i].tcp.SendData(packet);
                }
            }
        }

        private static void SendUDPData(int clientID, Packet packet)
        {
            packet.WriteLength();
            Server.clients[clientID].udp.SendData(packet);
        }

        private static void SendUDPDataToAll(Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].udp.SendData(packet);
            }
        }

        private static void SendUDPDataToAll(int expectClientID, Packet packet)
        {
            packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != expectClientID)
                {
                    Server.clients[i].udp.SendData(packet);
                }
            }
        }
        #endregion
    }

}