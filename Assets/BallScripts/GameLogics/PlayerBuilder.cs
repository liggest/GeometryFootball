using System;
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
                InitBarRigidbody(bar.gameObject.AddComponent<Rigidbody>());
                Animator ani = InitBarAnimator(bar.gameObject.AddComponent<Animator>());
                if(info.id == Clients.Client.instance.myID)
                {
                    bar.gameObject.AddComponent<HeroBarCollision>().InitAnimator(ani);
                }
                else
                {
                    bar.gameObject.AddComponent<Clients.BarCollision>().InitAnimator(ani);
                }
                bar.Init(StageObjectCategory.Dynamic, info.firstBar + barOffset);
                barOffset++;
            });
            Team team = TeamManager.instance.TryAddTeamByDescribe(info.teamDescribe);
            if (team != null)  //如果客户端新加了队伍，尝试寻找球门
            {
                TeamManager.instance.TryGetGoalFromCache(team);
            }
            TeamManager.instance.AddToTeam(obj, info.teamDescribe.id);

            ResourcesManager.LoadAndInstantiate("ChargeParticle", obj.transform,(GameObject g)=>
            {
                obj.InitParticle(g.GetComponent<ParticleSystem>());
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
                bar.gameObject.AddComponent<Servers.BarCollision>().InitCenter();
                InfoSender sender = bar.gameObject.AddComponent<InfoSender>();
                sender.sendLocal = true; //Bar发送local信息
                bar.Init(StageObjectCategory.Dynamic, info.firstBar + barOffset);
                barOffset++;
            });
            PlayerController controller = obj.gameObject.AddComponent<PlayerController>();
            controller.InitPlayer();
            controller.SetUltimate(info.playerType);
            TeamManager.instance.AddToTeam(obj, info.teamDescribe.id);
            return obj;
        }
        public override Player AfterInstantiate(GameObject obj, PlayerBuildInfo info)
        {
            Player player = base.AfterInstantiate(obj, info);
            player.playerType = info.playerType;
            player.InitText();
            player.myName.text = info.playerName;
            player.myName.color = info.teamDescribe.color;
            player.Score = info.score;
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

        public static Animator InitBarAnimator(Animator ani)
        {
            void SetController(RuntimeAnimatorController controller)
            {
                ani.runtimeAnimatorController = controller;
            }
            ResourcesManager.GetOrLoad<RuntimeAnimatorController>("BarController", SetController);
            return ani;
        }

        public override PlayerBuildInfo GenerateInfo(Player obj)
        {
            PlayerBuildInfo info = new PlayerBuildInfo
            {
                firstBar = obj.barList[0].id,
                playerType = obj.playerType,
                playerName = obj.myName.text,
                teamDescribe = obj.team.GetDescribe(),
                score = obj.Score
            };
            return SetBaseInfo(info, obj);
        }
    }
}

