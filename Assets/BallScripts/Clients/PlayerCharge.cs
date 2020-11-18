using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Utils;
using BallScripts.GameLogics;

namespace BallScripts.Clients
{
    public class PlayerCharge : MonoBehaviour
    {
        public Player player; 
        public ParticleSystem chargeParticle;
        public bool particleInited = false;
        public bool isPlay = false;
        float lastPower = 0;

        void Update()
        {

            if(lastPower != player.Power)
            {
                if (isPlay)
                {
                    chargeParticle.Stop();
                    isPlay = false;
                }
                else
                {
                    chargeParticle.Play();
                    isPlay = true;
                    //foreach (Bar bar in player.barList)
                    //{
                    //    bar.SetProgress(player.Power/player.maxPower);
                    //}
                    player.barList[0].SetProgress(player.Power / player.maxPower);
                }

                if (player.Power == 0)
                {
                    //foreach (Bar bar in player.barList)
                    //{
                    //    bar.SetEmission(false);
                    //    bar.SetProgress(0f);
                    //}
                    player.barList[0].SetEmission(false);
                    player.barList[0].SetProgress(0f);
                }

                lastPower = player.Power;
            }
            else
            {
                chargeParticle.Stop();
            }

            if (player.IsMaxPower)
            {
                //foreach (Bar bar in player.barList)
                //{
                //    bar.SetEmission(true);
                //}
                player.barList[0].SetEmission(true);
            }
        }

        public void Init(Player _player, ParticleSystem particle)
        {
            player = _player;
            chargeParticle = particle;
            chargeParticle.Stop();
            particleInited = true;
            Bar firtsBar = player.barList[0];
            for (int i = 0; i < player.barList.Count; i++)
            {
                player.barList[i].mr.material = firtsBar.mr.material;
            }
        }
    }
}

