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
                if (goal.TryScore(shootScore)) 
                {   
                    other.GetComponent<BaseStageObject>().ResetLocationInfo();
                    ServerSend.GoalScored(goal, shootScore);
                }
            }
        }
    }
}

