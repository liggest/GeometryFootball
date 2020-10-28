using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.Servers;

namespace BallScripts.GameLogics
{
    public class Player : BaseStageObject
    {
        public PlayerController controller;

        //public float barSpeed = 0.5f;
        //float step = 0;
        //bool isBarRotate = false;

        //List<Transform> barList = new List<Transform>();
        //List<Vector3> barPositionList = new List<Vector3>();

        private new void Start()
        {
            //foreach (Transform child in transform.GetComponentInChildren<Transform>())
            //{
            //    if (child.gameObject.layer == 8) //8 Bar
            //    {
            //        barList.Add(child);
            //        barPositionList.Add(child.localPosition);
            //    }
            //}
        }

        private void FixedUpdate()
        {
            
        }

        void Update()
        {


            //if (!isBarRotate && Input.GetKey(KeyCode.J))
            //{
            //    isBarRotate = true;
            //}

            ////让 Bar 转
            //if (isBarRotate)
            //{
            //    for (int i = 0; i < barList.Count; i++)
            //    {
            //        step += barSpeed * Time.deltaTime;
            //        //barList[i].position = Vector3.MoveTowards(barList[i].position, barPositionList[i], step);
            //        barList[i].localPosition = Vector3.Lerp(barPositionList[i], barPositionList[(i + 1) % barList.Count], step);
            //    }
            //    if (step > 1)
            //    {
            //        step = 0;
            //        Vector3 barPos0 = barPositionList[0];
            //        for (int i = 0; i < barList.Count - 1; i++)
            //        {
            //            barPositionList[i] = barPositionList[(i + 1) % (barList.Count)];
            //        }
            //        barPositionList[barList.Count - 1] = barPos0;
            //        isBarRotate = false;
            //    }

            //}

        }

    }
}


