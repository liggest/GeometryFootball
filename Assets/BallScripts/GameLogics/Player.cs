using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Servers;

namespace BallScripts.GameLogics
{
    public class Player : BaseStageObject
    {
        [HideInInspector]
        public PlayerController controller;
        public float maxPower = 100;
        private float power = 0;
        public float Power { get => power; set => power = Mathf.Clamp(value, 0, maxPower); }

        public List<Bar> barList = new List<Bar>();

        bool barInited = false;

        public string playerType = string.Empty;

        public Team team;

        public TextMesh myName;

        protected new void Start()
        {
            base.Start(); //BaseStageObject初始化（如果有的话）

            if (!barInited)
            {
                InitBars();
            }

        }
        
        public void InitBars()
        {
            barList.Clear();
            Bar lastBar = null;
            foreach (Bar child in transform.GetComponentsInChildren<Bar>())
            {
                barList.Add(child);
                if (lastBar)
                {
                    child.Previous = lastBar;
                    lastBar.Next = child;
                }
                lastBar = child;
            }
            lastBar.Next = barList[0];
            barList[0].Previous = lastBar;
        }

        public void InitText()
        {
            myName = transform.Find("Cube/MyName").GetComponent<TextMesh>();
        }

        public override void LastWord()
        {
            base.LastWord();
            //barList.ForEach((Bar bar) =>
            //{
            //    StageManager.instance.RemoveStageObject(bar.category, bar.id);
            //});
            //TeamManager.instance.RemoveFromTeam(this);
        }
    }
}


