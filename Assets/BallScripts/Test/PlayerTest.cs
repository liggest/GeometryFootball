using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Servers;
using BallScripts.GameLogics;

namespace BallScripts.Test
{
    public class PlayerTest : MonoBehaviour
    {
        PlayerController controller;
        float lastMoveValue = 0;
        float lastRotateValue = 0;
        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<PlayerController>();
        }

        // Update is called once per frame
        void Update()
        {
            float moveValue = Input.GetAxis("Vertical");
            if (moveValue != lastMoveValue)
            {
                controller.SetBuffer(InputType.move, moveValue);
                lastMoveValue = moveValue;
            }
            float rotateValue = Input.GetAxis("Horizontal");
            if (rotateValue != lastRotateValue)
            {
                controller.SetBuffer(InputType.rotate, rotateValue);
                lastRotateValue = rotateValue;
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                controller.SetBuffer(InputType.barRotate, 1);
            }
            else if (Input.GetKeyUp(KeyCode.J))
            {
                controller.SetBuffer(InputType.barRotate, 0);
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                controller.SetBuffer(InputType.charge, 1);
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
                controller.SetBuffer(InputType.charge, -1);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                controller.SetBuffer(InputType.ultimate, 1);
            }
        }
    }
}


