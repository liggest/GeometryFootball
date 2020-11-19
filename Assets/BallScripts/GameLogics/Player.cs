using System.Collections;
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


        protected new void Start()
        {
            base.Start(); //BaseStageObject初始化（如果有的话）

            if (!barInited)
            {
                InitBars();
            }

            //if (!particleInited)
            //{
            //    InitParticle();
            //}

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
            barInited = true;
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

        public static (Vector3,Quaternion) GetSpawnInfo(Team playerTeam)
        {
            Vector3 playerPos = playerTeam.GetSpawnPoint();
            Quaternion playerRot = Quaternion.LookRotation(Vector3.zero - new Vector3(playerPos.x, 0, playerPos.z));
            return (playerPos, playerRot);
        }

        public static void AddPlayerPrefix(GameObject obj,Player player)
        {
            string prefix = $"[[{player.id}]]";
            obj.name = prefix + obj.name;
        }

        public static bool TryGetPlayerByPrefix(string nameWithPrefix, out Player player)
        {
            if (!nameWithPrefix.StartsWith("[["))
            {
                player = null;
                return false;
            }
            string idstr = nameWithPrefix.Substring(2, nameWithPrefix.IndexOf(']') - 2);
            if(int.TryParse(idstr,out int id))
            {
                player = StageManager.instance.GetStageObject(StageObjectCategory.Player, id) as Player;
                return player;
            }
            player = null;
            return false;


        }

        public override void ResetLocationInfo()
        {
            base.ResetLocationInfo();
            if (team != null)
            {
                (Vector3 playerPos, Quaternion playerRot) = GetSpawnInfo(team);
                SetPosition(playerPos);
                SetRotation(playerRot);
            }
        }
    }
}


