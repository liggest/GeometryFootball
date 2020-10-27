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

        public float force = 50f;
        float maxSpeed = 12f;
        public float rotateSpeed = 160;

        private void Start()
        {
            player = GetComponent<Player>();
            player.controler = this;
            rd = GetComponent<Rigidbody>();

            RefreshBuffer();
        }

        public void SetBuffer(InputType key,float value)
        {
            buffer[key] = value;
        }

        public void RefreshBuffer()
        {
            foreach (InputType key in Enum.GetValues(iptType))
            {
                buffer[key] = 0;
            }
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

            RefreshBuffer();
        }


    }

}

