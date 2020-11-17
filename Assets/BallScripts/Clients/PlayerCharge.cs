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
        float lastPower = -1;

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
                }
                lastPower = player.Power;
            }
            else
            {
                chargeParticle.Stop();
            }

            if (player.IsMaxPower)
            {
                ChangeBarColor(new Color(0.98f, 0.8f, 0.466f));
            }
            if(player.Power == 0)
            {
                ChangeBarColor(new Color(1f, 1f, 1f));
            }
        }

        public void Init(Player _player, ParticleSystem particle)
        {
            player = _player;
            chargeParticle = particle;
            chargeParticle.Stop();
            particleInited = true;
        }

        public void ChangeBarColor(Color targetColor)
        {
            foreach (Bar bar in player.barList)
            {
                bar.mr.material.color = targetColor;
            }
        }
    }
}

