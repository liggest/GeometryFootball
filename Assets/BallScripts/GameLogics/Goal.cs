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

        public Rect spawnZone;
        public float spawnHeight = 1.2f;

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
            ResetScore();
            //team.Score -= Score;
            team = null;
            meshRenderer.material.color = initColor;
            unbindGoals.Add(name, this);
            StageManager.instance.RemoveStageObject(category, id);
            Actions.GoalResetAction?.Invoke(this);
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

        public Vector3 GetSpawnPoint()
        {
            return new Vector3(Random.Range(spawnZone.xMin, spawnZone.xMax), spawnHeight, Random.Range(spawnZone.yMin, spawnZone.yMax));
        }

        private void OnDestroy()
        {
            if (unbindGoals.ContainsKey(name))
            {
                unbindGoals.Remove(name);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 xyMin = new Vector3(spawnZone.xMin, spawnHeight, spawnZone.yMin);
            Vector3 xyMax = new Vector3(spawnZone.xMax, spawnHeight, spawnZone.yMax);
            Vector3 xMiny = new Vector3(spawnZone.xMin, spawnHeight, spawnZone.yMax);
            Vector3 yMinx = new Vector3(spawnZone.xMax, spawnHeight, spawnZone.yMin);
            Color origin = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(xyMin, xMiny);
            Gizmos.DrawLine(xMiny, xyMax);
            Gizmos.DrawLine(xyMax, yMinx);
            Gizmos.DrawLine(yMinx, xyMin);
            Gizmos.color = origin;
        }

    }
}


