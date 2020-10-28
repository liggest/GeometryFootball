using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.GameLogics
{
    public class Bar : BaseStageObject
    {
        public Vector3 original;

        public Bar Previous { get; set; }
        public Bar Next { get; set; }

        protected new void Start()
        {

            base.Start();

            original = transform.localPosition;
        }
    }

}
