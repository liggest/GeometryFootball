using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Clients;
using BallScripts.GameLogics;
using BallScripts.Utils;

namespace BallScripts.Servers
{
    public class PlayerController : MonoBehaviour
    {
        Dictionary<InputType, float> buffer = new Dictionary<InputType, float>();
        Player player;
        Rigidbody rd;
        Type iptType = typeof(InputType);

        public float force = 250f;
        float maxSpeed = 12f;
        public float rotateSpeed = 160;

        private void Start()
        {
            player = GetComponent<Player>();
            player.controler = this;
            rd = GetComponent<Rigidbody>();

            foreach (InputType key in Enum.GetValues(iptType))
            {
                buffer[key] = 0;
            }
        }

        public void SetBuffer(InputType key,float value)
        {
            buffer[key] = value;
        }

        public void RefreshBuffer()
        {
            buffer[InputType.barRotate] = 0;
            buffer[InputType.ultimate] = 0;
        }

        private void FixedUpdate()
        {
            //移动
            float h = buffer[InputType.rotate];
            float accelerator = buffer[InputType.move];
            rd.AddForce(accelerator * transform.forward * force);
            if (rd.velocity.magnitude >= maxSpeed)
            {
                rd.velocity = rd.velocity.normalized * maxSpeed;
            }

            transform.Rotate(0, h * rotateSpeed * Time.deltaTime, 0, Space.Self);

            buffer[InputType.barRotate] = 0;
            buffer[InputType.ultimate] = 0;
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
            lastPlayer.controler.SetBuffer(key, value);
        }


    }

}

