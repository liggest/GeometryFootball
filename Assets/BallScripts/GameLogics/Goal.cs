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

        private int score = 0;
        public int Score { get => score; set => score = value > 0 ? value : 0; }

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
            //team.AddSocre(Score);
            Debug.Log($"球门{id} 绑定了 队伍{team.id}   球门的 {Score}分 进入了队伍");
        }

        public void UnbindFromTeam()
        {
            team.Score -= Score;
            team = null;
            meshRenderer.material.color = initColor;
            unbindGoals.Add(name, this);
        }

        /*
        public bool TryScore(int value)
        {
            AddSocre(value);
            if (team != null)
            {
                //team.AddSocre(value);
                return true;
            }
            return false;
        }
        */

        public void AddSocre(int value)
        {
            Score += value;
        }

        public void ResetScore()
        {
            Score = 0;
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


