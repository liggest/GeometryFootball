using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.GameLogics
{
    public class Ball : BaseStageObject
    {
        public Player lastPlayer;

        private void OnCollisionEnter(Collision collision)
        {
            Transform target = null;
            if (collision.collider.CompareTag("Player")) //Player最外层
            {
                target = collision.transform;
            }
            else if (collision.collider.transform.parent && collision.collider.transform.parent.CompareTag("Player")) //Player模型层、Player所属物
            {
                target = collision.transform.parent;
            }
            else if (Player.TryGetPlayerByPrefix(collision.gameObject.name,out Player player)) //Player所属物
            {
                lastPlayer = player;
            }
            else if (collision.collider.CompareTag("Bar")) //Bar
            {
                target = collision.transform.parent.parent;
            }
            if (target) 
            {
                if(!lastPlayer || lastPlayer.transform != target)
                {
                    lastPlayer = target.GetComponent<Player>();
                    Debug.Log($"[Ball] lastPlayer - {lastPlayer.id}");
                }
            }
        }
    }

}

