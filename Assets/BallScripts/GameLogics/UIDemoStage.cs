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

        int leftScore = 0;
        int rightScore = 0;

        private int leftTeamID = -1;
        private int rightTeamID = -1;

        private void Start()
        {
            leftScoreText.text = leftScore.ToString();
            rightScoreText.text = rightScore.ToString();

            Actions.TeamScoredAction += AddScore;
        }

        private void AddScore(Team team, int score)
        {
            if( leftTeamID == -1 || rightTeamID == -1)
            {
                if (!team.HasNoGoal)
                {
                    Goal teamGoal = team.Goals[0];

                    if(teamGoal.transform.position.x < 0)
                    {
                        leftTeamID = team.id;
                    }
                    else if(teamGoal.transform.position.x > 0)
                    {
                        rightTeamID = team.id;
                    }
                }
     
            }
            if(team.id == leftTeamID)
            {
                leftScore += score;
                leftScoreText.text = leftScore.ToString();
            }
            else if(team.id == rightTeamID)
            {
                rightScore += score;
                rightScoreText.text = rightScore.ToString();
            }
        }

    }
}

