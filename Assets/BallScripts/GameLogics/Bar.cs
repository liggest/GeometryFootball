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
        public MeshRenderer mr;

        protected new void Start()
        {

            base.Start();

            original = transform.localPosition;
            mr = GetComponent<MeshRenderer>();
        }

        public void SetProgress(float value)
        {
            value = value * 0.0138f + 0.4931f;  //范围应该在0.4931到0.5069
            mr.material.SetFloat("_Progress", value);
        }

        public void SetEmission(bool isMax)
        {
            if (isMax)
            {
                mr.material.SetColor("_EmissionColor", mr.material.GetColor("_Color2"));
            }
            else
            {
                mr.material.SetColor("_EmissionColor", Color.black);
            }
        }
    }

}
