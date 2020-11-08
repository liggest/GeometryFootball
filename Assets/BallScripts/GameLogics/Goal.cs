using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.GameLogics
{
    public class Goal : BaseStageObject
    {
        MeshRenderer meshRenderer;
        Color initColor = Color.white;
        protected new void Start()
        {
            base.Start(); //BaseStageObject初始化（如果有的话）

            meshRenderer = GetComponent<MeshRenderer>();
            initColor = meshRenderer.material.color;
        }

        Team team;

        public bool HasTeam { get => team != null; }
        public void BindToTeam(Team goalTeam)
        {
            team = goalTeam;
            meshRenderer.material.color = team.teamColor;
        }

        public void UnbindFromTeam()
        {
            team = null;
            meshRenderer.material.color = initColor;
        }

        public void Score(int value)
        {
            if (team != null)
            {
                team.Score += value;
            }
        }
    }
}


