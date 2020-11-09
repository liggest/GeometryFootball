using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BallScripts.GameLogics
{
    public class Goal : BaseStageObject
    {
        public static Dictionary<string, Goal> unbindGoals = new Dictionary<string, Goal>();

        MeshRenderer meshRenderer;
        Color initColor = Color.white;

        private void Awake()
        {
            unbindGoals.Add(name, this);
        }

        protected new void Start()
        {
            base.Start(); //BaseStageObject初始化（如果有的话）

            meshRenderer = GetComponent<MeshRenderer>();
            initColor = meshRenderer.material.color;
        }

        Team team;

        public int TeamID { get => HasTeam ? team.id : -1; }
        public bool HasTeam { get => team != null; }
        public void BindToTeam(Team goalTeam)
        {
            team = goalTeam;
            meshRenderer.material.color = team.teamColor;
            unbindGoals.Remove(name);
        }

        public void UnbindFromTeam()
        {
            team = null;
            meshRenderer.material.color = initColor;
            unbindGoals.Add(name, this);
        }

        public void Score(int value)
        {
            if (team != null)
            {
                team.Score += value;
            }
        }

        private void OnDestroy()
        {
            if (unbindGoals.ContainsKey(name))
            {
                unbindGoals.Remove(name);
            }
        }
    }
}


