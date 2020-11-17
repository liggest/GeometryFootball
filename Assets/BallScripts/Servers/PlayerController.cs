using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;
using NMM = BallScripts.GameLogics.NetworkMarkerManager;
using Pair = BallScripts.GameLogics.StageObjectPair;

namespace BallScripts.Servers
{
    public class PlayerController : MonoBehaviour
    {
        Dictionary<InputType, float> buffer = new Dictionary<InputType, float>();
        Player player;
        Rigidbody rd;
        Type iptType = typeof(InputType);

        public float force = 250f;
        float originMaxSpeed = 12f;
        float chargeMaxSpeed = 3f;
        float currentMaxSpeed = 12f;
        public float rotateSpeed = 160;
        float rotateSpeedFactor = 0;

        public float barSpeed = 0.5f;
        float barSpeedFactor = 0;
        bool isBarRotate = false;
        float step = 0;

        public BaseUltimate ultimate;

        private void Start()
        {
            rotateSpeedFactor = rotateSpeed * Time.fixedDeltaTime;
            barSpeedFactor = barSpeed * Time.fixedDeltaTime;

            InitPlayer();
            rd = GetComponent<Rigidbody>();

            foreach (InputType key in Enum.GetValues(iptType))
            {
                buffer[key] = 0;
                //Debug.Log(key);
            }
        }

        public void SetBuffer(InputType key,float value)
        {
            //Debug.Log($"{player.id} {key}");
            buffer[key] = value;
            //if (key == InputType.move)
            //{
            //    Debug.Log($"InputType.move:{buffer[InputType.move]}");
            //}
        }

        public void RefreshBuffer()
        {
            //Debug.Log($"{player.id} Refresh");
            //buffer[InputType.barRotate] = 0;
            buffer[InputType.ultimate] = 0;
        }

        public void InitPlayer()
        {
            if (player != null)
            {
                return;
            }
            player = GetComponent<Player>();
            player.controller = this;
        }

        public void SetUltimate(string playerType)
        {
            if (playerType == "Demo")
            {
                SetUltimate(new UltimateCube());
            }
        }

        public void SetUltimate(BaseUltimate _ultimate)
        {
            ultimate = _ultimate;
            ultimate.Init(player);
        }

        private void FixedUpdate()
        {
            //移动
            float h = buffer[InputType.rotate];
            float accelerator = buffer[InputType.move];
            //Debug.Log(buffer[InputType.move]);
            rd.AddForce(accelerator * transform.forward * force);
            if (rd.velocity.magnitude >= currentMaxSpeed)
            {
                rd.velocity = rd.velocity.normalized * currentMaxSpeed;
            }

            transform.Rotate(0, h * rotateSpeedFactor, 0, Space.Self);

            //鼠标
            //旧
            //Vector3 rDir = new Vector3(buffer[InputType.mouseX], 0, buffer[InputType.mouseY]);
            //if(rDir != Vector3.zero)
            //{
            //    Vector3 nDir = rDir.normalized;
            //    transform.forward = nDir;
            //}

            //新（平滑旋转）
            Rotate(transform, buffer[InputType.mouseX], buffer[InputType.mouseY], 0.2f);


            //转bar
            if (!isBarRotate && buffer[InputType.barRotate] > 0)
            {
                isBarRotate = true;
            }

            if (isBarRotate)
            {
                for (int i = 0; i < player.barList.Count; i++)
                {
                    Bar current = player.barList[i];
                    step += barSpeedFactor;
                    current.transform.localPosition = Vector3.Lerp(current.original, current.Next.original, step);
                }
                if (step > 1)
                {
                    step = 0;
                    Vector3 firstOriginal = player.barList[0].original;
                    for (int i = 0; i < player.barList.Count - 1; i++)
                    {
                        Bar current = player.barList[i];
                        current.original = current.Next.original;
                    }
                    player.barList[0].Previous.original = firstOriginal;
                    isBarRotate = false;
                }
            }

            if (buffer[InputType.charge] > 0)
            {
                player.AddPower(player.powerPerSecond * Time.fixedDeltaTime);
                ServerSend.StageObjectInfo(new Pair { category = player.category, id = player.id }, nameof(player.Power), player.Power);
                if (player.IsMaxPower)
                {
                    Debug.Log($"玩家{player.id} 洪荒之力已满 大招充能完毕");
                    player.ChangeBarColor(new Color(0.98f, 0.8f, 0.466f));
                }
                currentMaxSpeed = chargeMaxSpeed;
                player.chargeParticle.Play();
            }
            else
            {
                if (currentMaxSpeed != originMaxSpeed)
                {
                    currentMaxSpeed = originMaxSpeed;
                }
                player.chargeParticle.Stop();
            }

            //大招
            if (player.IsMaxPower && buffer[InputType.ultimate] > 0)
            {
                if (ultimate != null && !ultimate.IsOn)
                {
                    ultimate.Enter();
                    player.ChangeBarColor(new Color(1f, 1f, 1f));
                }
                player.ResetPower();
                ServerSend.StageObjectMethod(new Pair { category = player.category, id = player.id }, nameof(player.ResetPower));
            }

            if (ultimate!=null && ultimate.IsOn)
            {
                ultimate.FixedUpdate();
            }

            RefreshBuffer();
        }

        private void Update()
        {
            if (ultimate != null && ultimate.IsOn)
            {
                ultimate.Update();
            }
            /*
            if (Input.GetKeyDown(KeyCode.Z))
            {
                player.barList[2].TestValue = UnityEngine.Random.Range(0, 100);
                Debug.Log(player.barList[2].TestValue);
                ServerSend.StageObjectInfo(new Pair { category = player.category, id = player.id }, NMM.GetRoute(nameof(player.barList), "[2]", "TestValue"), player.barList[2].TestValue);
                BaseStageObject ball = StageManager.instance.GetStageObject(StageObjectCategory.Ball, 1);
                ball.TestProp += UnityEngine.Random.Range(0, 100).ToString();
                Debug.Log(ball.TestProp);
                ServerSend.StageObjectInfo(new Pair { category = ball.category, id = ball.id }, NMM.GetRoute(nameof(ball.TestProp)), ball.TestProp);
                BaseStageObject goal = StageManager.instance.GetStageObject(StageObjectCategory.Goal, 1);
                ServerSend.StageObjectMethod(new Pair { category = goal.category, id = goal.id }, NMM.GetRoute(nameof(goal.TestMethod)), 3, "55", new object[] { "16", "28", 99, 3.0f });
            }
            */
        }

        static int lastID = -1;
        static Player lastPlayer;

        public static void SetClientBuffer(int clientID, InputType key, float value)
        {
            if (clientID != lastID)
            {
                lastID = clientID;
                lastPlayer = StageManager.instance.GetStageObject(StageObjectCategory.Player, clientID) as Player;
            }
            lastPlayer.controller.SetBuffer(key, value);
        }

        public static void ResetLast()//每次换场景一定要重置，不然lastPlayer可能引用之前场景的内容，造成输入不响应的bug
        {
            lastID = -1;
            lastPlayer = null;
        }


        public void Rotate(Transform transform, float horizontal, float vertical, float fRotateSpeed)
        {
            Vector3 targetDir = new Vector3(horizontal, 0, vertical);
            if (targetDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, fRotateSpeed);
            }
        }

    }

}

