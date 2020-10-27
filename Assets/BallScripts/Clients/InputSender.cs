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
        List<InputHolder> inputBuffer = new List<InputHolder>();
        private void FixedUpdate()
        {
            inputBuffer.Clear();
            float moveValue = Input.GetAxis("Vertical");
            if (moveValue != 0)
            {
                inputBuffer.Add(new InputHolder { key = InputType.move, value = moveValue });
            }
            float rotateValue = Input.GetAxis("Horizontal");
            if (rotateValue != 0)
            {
                inputBuffer.Add(new InputHolder { key = InputType.rotate, value = rotateValue });
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

            }
        }
    }

}

