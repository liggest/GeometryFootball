using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.Clients
{
    public class HeroBarCollision : BarCollision
    {
        public float chromaticIntensity;

        private new void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
            PPController.instance.ChromaticFadeIn();
        }
    }
}

