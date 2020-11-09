using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.Clients
{
    public class BarCollision : MonoBehaviour
    {
        Animator ani;

        private void Start()
        {
            if (ani == null)
            {
                InitAnimator(GetComponent<Animator>());
            }
            //ani = GetComponent<Animator>();
        }

        public void InitAnimator(Animator animator)
        {
            ani = animator;
        }

        protected void OnCollisionEnter(Collision collision)
        {
            ani?.Play("Collision");
        }
    }
}

