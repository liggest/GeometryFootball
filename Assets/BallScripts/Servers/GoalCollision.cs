using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;

namespace BallScripts.Servers
{
    public class GoalCollision : MonoBehaviour
    {
        public int shootScore = 1;
        Goal goal;
        private void Start()
        {
            if (!goal)
            {
                InitGoal(GetComponent<Goal>());
            }
        }

        public void InitGoal(Goal _goal)
        {
            goal = _goal;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ball"))
            {
                Ball ball = other.GetComponent<Ball>();
                if (ball.lastPlayer)
                {
                    int score = shootScore;
                    if (ball.lastPlayer.team.id == goal.TeamID)
                    {
                        score = -score;
                    }
                    ball.lastPlayer.AddSocre(score);
                    goal.AddSocre(score);
                    ServerSend.PlayerScored(ball.lastPlayer, goal, score);
                }
                ball.ResetLocationInfo();
            }
        }
    }
}

