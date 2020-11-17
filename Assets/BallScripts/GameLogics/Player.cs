﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Servers;

namespace BallScripts.GameLogics
{
    [NetworkClass]
    public class Player : BaseStageObject
    {
        [HideInInspector]
        public PlayerController controller;
        public float maxPower = 100;
        public float powerPerSecond = 5;
        private float power = 0;

        [NetworkMarker(nameof(Power))]
        public float Power { get => power; set => power = Mathf.Clamp(value, 0, maxPower); }
        public bool IsMaxPower { get => power == maxPower; }


        private int score = 0;
        public int Score { get => score; set => score = value > 0 ? value : 0; }

        public List<Bar> barList = new List<Bar>();

        bool barInited = false;

        public string playerType = string.Empty;

        public Team team;

        public TextMesh myName;

        public ParticleSystem chargeParticle;

        bool particleInited = false;


        protected new void Start()
        {
            base.Start(); //BaseStageObject初始化（如果有的话）

            if (!barInited)
            {
                InitBars();
            }

            if (!particleInited)
            {
                InitParticle();
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

        public void ChangeBarColor(Color targetColor)
        {
            foreach (Bar bar in barList)
            {
                bar.mr.material.color = targetColor;
            }
        }

        public void InitParticle()
        {
            //BaseBuildInfo info = new BaseBuildInfo
            //{
            //    category = StageObjectCategory.Other,
            //    id = StageManager.instance.GetMaxID(StageObjectCategory.Other) + 1,
            //    prefabName = "ChargeParticle",
            //};
            //GameManager.instance.SpawnStageObject(info, BuildType.Client).transform.SetParent(transform);
            chargeParticle = transform.Find("ChargeParticle").GetComponent<ParticleSystem>();
            chargeParticle.Stop();
        }

            public void InitText()
        {
            myName = transform.Find("Cube/MyName").GetComponent<TextMesh>();
        }

        public void AddPower(float value)
        {
            Power += value;
        }
        [NetworkMarker(nameof(ResetPower))]
        public void ResetPower()
        {
            Power = 0;
        }

        public void AddSocre(int value)
        {
            Score += value;
            team?.AddSocre(value);
        }

        public void ResetScore()
        {
            team?.AddSocre(-Score);
            Score = 0;
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


