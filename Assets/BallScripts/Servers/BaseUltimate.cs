using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.Servers
{
    public abstract class BaseUltimate
    {
        public abstract void Init();
        public abstract void FixedUpdate();
        public abstract void Update();
        public virtual void Enter()
        {
            isOn = true;
        }
        public virtual void Exit()
        {
            isOn = false;
        }
        private bool isOn = false;
        public bool IsOn { get => isOn; }
    }
}

