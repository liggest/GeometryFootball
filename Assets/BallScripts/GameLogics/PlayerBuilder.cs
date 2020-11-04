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
                Camera.main.gameObject.AddComponent<CameraTrack>().trackPlayer = obj.transform;
            }
            obj.InitBars();
            int barOffset = 0;
            obj.barList.ForEach((Bar bar) =>
            {
                bar.Init(StageObjectCategory.Dynamic, info.firstBar + barOffset);
                barOffset++;
            });
            return obj;
        }

        public override Player BuildInServer(Player obj, PlayerBuildInfo info)
        {
            InitPlayerRigidbody(obj.gameObject.AddComponent<Rigidbody>());
            obj.gameObject.AddComponent<InfoSender>();
            obj.InitBars();
            int barOffset = 0;
            obj.barList.ForEach((Bar bar) =>
            {
                InitBarRigidbody(bar.gameObject.AddComponent<Rigidbody>());
                bar.gameObject.AddComponent<BarCollision>();
                InfoSender sender = bar.gameObject.AddComponent<InfoSender>();
                sender.sendLocal = true; //Bar发送local信息
                bar.Init(StageObjectCategory.Dynamic, info.firstBar + barOffset);
                barOffset++;
            });
            PlayerController controller = obj.gameObject.AddComponent<PlayerController>();
            controller.InitPlayer();
            controller.SetUltimate(info.playerType);
            return obj;
        }
        public override Player AfterInstantiate(GameObject obj, PlayerBuildInfo info)
        {
            Player player = base.AfterInstantiate(obj, info);
            player.playerType = info.playerType;
            return player;
        }
        public override bool CheckInfo(BaseBuildInfo info)
        {
            return info is PlayerBuildInfo;
        }
        public static Rigidbody InitPlayerRigidbody(Rigidbody rig)
        {
            rig.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rig.mass = 5;
            rig.angularDrag = 0.1f;
            rig.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            return rig;
        }

        public static Rigidbody InitBarRigidbody(Rigidbody rig)
        {
            //rig.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rig.angularDrag = 0;
            rig.useGravity = false;
            rig.constraints = RigidbodyConstraints.FreezeAll;
            return rig;
        }

        public override PlayerBuildInfo GenerateInfo(Player obj)
        {
            PlayerBuildInfo info = new PlayerBuildInfo
            {
                firstBar = obj.barList[0].id,
                playerType = obj.playerType
            };
            return SetBaseInfo(info, obj);
        }
    }
}

