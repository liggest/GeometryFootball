using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BallScripts.GameLogics
{
    public class UIDemoStage : MonoBehaviour
    {
        public Text leftScoreText;
        public Text rightScoreText;

        Team leftTeam;
        Team rightTeam;

        private void Start()
        {
            if (leftTeam != null)
            {
                leftScoreText.text = leftTeam.Score.ToString();
            }
            if (rightTeam != null)
            {
                rightScoreText.text = rightTeam.Score.ToString();
            }
            Actions.TeamScoredAction += AddScore;
        }

        private void AddScore(Team team, int score)
        {
            if( leftTeam == null || rightTeam == null)
            {
                if (!team.HasNoGoal)
                {
                    Goal teamGoal = team.Goals[0];

                    if(teamGoal.transform.position.x < 0)
                    {
                        leftTeam = team;
                    }
                    else if(teamGoal.transform.position.x > 0)
                    {
                        rightTeam = team;
                    }
                }

            }
            if (leftTeam != null && team.id == leftTeam.id)
            {
                //leftScore += score;
                leftScoreText.text = leftTeam.Score.ToString();
            }
            else if (rightTeam != null && team.id == rightTeam.id)
            {
                //rightScore += score;
                rightScoreText.text = rightTeam.Score.ToString();
            }
        }

        private void OnDestroy()
        {
            Actions.TeamScoredAction -= AddScore;
        }

        private void OnApplicationQuit()
        {
            OnDestroy();
        }

    }
}

