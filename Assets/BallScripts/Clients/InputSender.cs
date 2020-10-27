using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Utils;

namespace BallScripts.Clients
{
    public enum InputType
    {
        move = 1,
        rotate,
        barRotate,
        charge, //得传状态
        ultimate,
    }

    public struct InputHolder
    {
        public InputType key;
        public float value;
    }

    public class InputSender : Singleton<InputSender>
    {
        float lastMoveValue = 0;
        float lastRotateValue = 0;

        List<InputHolder> inputBuffer = new List<InputHolder>();
        private void FixedUpdate()
        {
            inputBuffer.Clear();
            float moveValue = Input.GetAxis("Vertical");
            if (moveValue != lastMoveValue)
            {
                inputBuffer.Add(new InputHolder { key = InputType.move, value = moveValue });
                lastMoveValue = moveValue;
            }
            float rotateValue = Input.GetAxis("Horizontal");
            if (rotateValue != lastRotateValue)
            {
                inputBuffer.Add(new InputHolder { key = InputType.rotate, value = rotateValue });
                lastRotateValue = rotateValue;
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                inputBuffer.Add(new InputHolder { key = InputType.barRotate, value = 0 });
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                inputBuffer.Add(new InputHolder { key = InputType.charge, value = 1 });
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
                inputBuffer.Add(new InputHolder { key = InputType.charge, value = -1 });
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                inputBuffer.Add(new InputHolder { key = InputType.ultimate, value = 0 });
            }
            if (inputBuffer.Count > 0)
            {
                ClientSend.InputPacket(inputBuffer);
            }
        }
    }

}

