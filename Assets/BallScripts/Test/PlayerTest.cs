using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Servers;
using BallScripts.GameLogics;

namespace BallScripts.Test
{
    public class PlayerTest : MonoBehaviour
    {
        public string playerType = "Demo";

        PlayerController controller;
        float lastMoveValue = 0;
        float lastRotateValue = 0;
        // Start is called before the first frame update
        void Start()
        {
            GameManager.instance.ToString();
            controller = GetComponent<PlayerController>();
            controller.InitPlayer();
            controller.SetUltimate(playerType);
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
            if (Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0))
            {
                controller.SetBuffer(InputType.barRotate, 1);
            }
            else if (Input.GetKeyUp(KeyCode.J) || Input.GetMouseButtonUp(0))
            {
                controller.SetBuffer(InputType.barRotate, 0);
            }
            if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(2))
            {
                controller.SetBuffer(InputType.charge, 1);
            }
            if (Input.GetKeyUp(KeyCode.K) || Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(2))
            {
                controller.SetBuffer(InputType.charge, -1);
            }
            if (Input.GetKeyDown(KeyCode.L) || Input.GetMouseButtonDown(1))
            {
                controller.SetBuffer(InputType.ultimate, 1);
            }

            //鼠标部分
            Vector3 v3 = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = v3.z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            controller.SetBuffer(InputType.mouseX, (worldPos.x - transform.localPosition.x) * 0.1f);
            controller.SetBuffer(InputType.mouseY, (worldPos.z - transform.localPosition.z) * 0.1f);
        }
    }
}


