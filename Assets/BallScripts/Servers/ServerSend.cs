using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Utils;
using BallScripts.GameLogics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

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

        public static void ClientConnectionRefused(int clientID,string msg)
        {
            using (Packet packet = new Packet((int)ServerPackets.ClientConnectionRefused))
            {
                packet.Write(msg);
                //SendUDPData(clientID, packet);
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

        public static void StageObjectPositions(List<Vector3Holder> posList)
        {
            SendPositions(ServerPackets.StageObjectPositions, posList);
        }
        public static void StageObjectPositions(int clientID, List<Vector3Holder> posList)
        {
            SendPositions(ServerPackets.StageObjectPositions, posList, clientID);
        }

        public static void StageObjectLocalPositions(List<Vector3Holder> localPosList)
        {
            SendPositions(ServerPackets.StageObjectLocalPositions, localPosList);
        }

        static void SendPositions(ServerPackets packetID, List<Vector3Holder> posList, int clientID = -1)//如果 clientID>0 则只向该id发送
        {
            using (Packet packet = new Packet((int)packetID))
            {
                foreach (Vector3Holder holder in posList)
                {
                    packet.Write((int)holder.category);
                    packet.Write(holder.id);
                    packet.Write(holder.vect);
                }
                if (clientID > 0) 
                {
                    SendUDPData(clientID, packet);
                }
                else
                {
                    SendUDPDataToAll(packet);
                }
            }
        }

        public static void StageObjectRotations(List<QuaternionHolder> rotList)
        {
            SendRotations(ServerPackets.StageObjectRotations, rotList);
        }
        public static void StageObjectRotations(int clientID, List<QuaternionHolder> rotList)
        {
            SendRotations(ServerPackets.StageObjectRotations, rotList, clientID);
        }
        public static void StageObjectLocalRotations(List<QuaternionHolder> localRotList)
        {
            SendRotations(ServerPackets.StageObjectLocalRotations, localRotList);
        }

        static void SendRotations(ServerPackets packetID, List<QuaternionHolder> rotList, int clientID = -1)//如果 clientID>0 则只向该id发送
        {
            using (Packet packet = new Packet((int)packetID))
            {
                foreach (QuaternionHolder holder in rotList)
                {
                    packet.Write((int)holder.category);
                    packet.Write(holder.id);
                    packet.Write(holder.quat);
                }
                if (clientID > 0)
                {
                    SendUDPData(clientID, packet);
                }
                else
                {
                    SendUDPDataToAll(packet);
                }
            }
        }

        public static void StageObjectSpawned<T>(int clientID, T info) where T : BaseBuildInfo
        {
            using (Packet packet = new Packet((int)ServerPackets.StageObjectSpawned))
            {
                Serialize(info, packet);
                SendTCPData(clientID, packet);
            }
        }

        public static void StageObjectSpawned<T>(T info) where T : BaseBuildInfo
        {
            using (Packet packet = new Packet((int)ServerPackets.StageObjectSpawned))
            {
                Serialize(info, packet);
                SendTCPDataToAll(packet);
            }
        }

        static void Serialize<T>(T obj, Packet packet)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.SurrogateSelector = SurrogateManager.GetSurrogateSelector();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                stream.Flush();
                packet.Write(stream.GetBuffer());
            }
        }

        public static void StageObjectDespawned(StageObjectCategory category, int id)
        {
            StageObjectDespawned(new List<StageObjectPair> {
                new StageObjectPair { category = category, id = id }
            });
        }

        public static void StageObjectDespawned(int clientID, StageObjectCategory category, int id)
        {
            StageObjectDespawned(clientID, new List<StageObjectPair> {
                new StageObjectPair { category = category, id = id }
            });
        }

        public static void StageObjectDespawned(List<StageObjectPair> objs)
        {
            using (Packet packet = new Packet((int)ServerPackets.StageObjectDespawned))
            {
                Pairs2Packet(objs, packet);
                SendTCPDataToAll(packet);
            }
        }

        public static void StageObjectDespawned(int clientID, List<StageObjectPair> objs)
        {
            using (Packet packet = new Packet((int)ServerPackets.StageObjectDespawned))
            {
                Pairs2Packet(objs, packet);
                SendTCPData(clientID, packet);
            }
        }
        public static void StageObjectRemoved(List<StageObjectPair> objs) 
        {
            //和上面的Despawned不同，这里只调用StageManager.instance.RemoveStageObject
            //而不是GameManager.instance.DespawnStageObject
            using (Packet packet = new Packet((int)ServerPackets.StageObjectRemoved))
            {
                Pairs2Packet(objs, packet);
                SendTCPDataToAll(packet);
            }
        }
        public static void StageObjectRemoved(int clientID, List<StageObjectPair> objs)
        {
            using (Packet packet = new Packet((int)ServerPackets.StageObjectRemoved))
            {
                Pairs2Packet(objs, packet);
                SendTCPData(clientID, packet);
            }
        }

        static void Pairs2Packet(List<StageObjectPair> pairs,Packet packet)
        {
            foreach (StageObjectPair pair in pairs)
            {
                packet.Write((int)pair.category);
                packet.Write(pair.id);
            }
        }

        public static void StageObjectInfo(StageObjectPair pair, string route,object value)
        {
            using (Packet packet = new Packet((int)ServerPackets.StageObjectInfo))
            {
                packet.Write((int)pair.category);
                packet.Write(pair.id);
                packet.Write(route);
                packet.Write(value);
                SendTCPDataToAll(packet);
            }
        }
        public static void StageObjectMethod(StageObjectPair pair, string route, params object[] parameters)
        {
            using (Packet packet = new Packet((int)ServerPackets.StageObjectMethod))
            {
                packet.Write((int)pair.category);
                packet.Write(pair.id);
                packet.Write(route);
                for (int i = 0; i < parameters.Length; i++)
                {
                    packet.Write(parameters[i]);
                }
                SendTCPDataToAll(packet);
            }
        }

        public static void GoalScored(Goal goal, int value)
        {
            using (Packet packet = new Packet((int)ServerPackets.GoalScored))
            {
                packet.Write(goal.id);
                packet.Write(goal.Score);
                packet.Write(value);
                SendTCPDataToAll(packet);
            }
        }

        public static void TeamLeft(int playerID)
        {
            using (Packet packet = new Packet((int)ServerPackets.TeamLeft))
            {
                packet.Write(playerID);
                SendTCPDataToAll(packet);
            }
        }


        /*
        public static void PlayerSpawned(int clientID, string prefabName)
        {
            using (Packet packet = new Packet((int)ServerPackets.PlayerSpawned))
            {
                packet.Write(clientID);
                packet.Write(prefabName);
                SendTCPDataToAll(packet);
            }
        }
        */

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
            foreach (int cid in Server.onlineClients.Values)
            {
                Server.clients[cid].tcp.SendData(packet);
            }
            //for (int i = 1; i <= Server.MaxPlayers; i++)
            //{
            //    Server.clients[i].tcp.SendData(packet);
            //}
        }

        private static void SendTCPDataToAll(int expectClientID, Packet packet)
        {
            packet.WriteLength();
            foreach (int cid in Server.onlineClients.Values)
            {
                if (cid != expectClientID)
                {
                    Server.clients[cid].tcp.SendData(packet);
                }
            }
            //for (int i = 1; i <= Server.MaxPlayers; i++)
            //{
            //    if (i != expectClientID)
            //    {
            //        Server.clients[i].tcp.SendData(packet);
            //    }
            //}
        }

        private static void SendUDPData(int clientID, Packet packet)
        {
            packet.WriteLength();
            Server.clients[clientID].udp.SendData(packet);
        }

        private static void SendUDPDataToAll(Packet packet)
        {
            packet.WriteLength();
            foreach (int cid in Server.onlineClients.Values)
            {
                Server.clients[cid].udp.SendData(packet);
            }
            //for (int i = 1; i <= Server.MaxPlayers; i++)
            //{
            //    Server.clients[i].udp.SendData(packet);
            //}
        }

        private static void SendUDPDataToAll(int expectClientID, Packet packet)
        {
            packet.WriteLength();
            foreach (int cid in Server.onlineClients.Values)
            {
                if (cid != expectClientID)
                {
                    Server.clients[cid].udp.SendData(packet);
                }
            }
            //for (int i = 1; i <= Server.MaxPlayers; i++)
            //{
            //    if (i != expectClientID)
            //    {
            //        Server.clients[i].udp.SendData(packet);
            //    }
            //}
        }
        #endregion
    }

}