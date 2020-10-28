using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;

namespace BallScripts.Servers
{
    public class InfoSender : MonoBehaviour
    {
        public bool sendLocal = false;
        //开启后将转而发送local位置、旋转
        //local位置、旋转 和 全局位置、旋转，逻辑上只需要同步其一

        Vector3 lastPosition;
        Quaternion lastRotation;
        BaseStageObject target;

        // Start is called before the first frame update
        void Start()
        {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            target = GetComponent<BaseStageObject>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (sendLocal)
            {
                if (transform.localPosition != lastPosition)
                {
                    lastPosition = transform.localPosition;
                    InfoBuffer.AddLocal(target.category, target.id, lastPosition);
                }
                if (transform.localRotation != lastRotation)
                {
                    lastRotation = transform.localRotation;
                    InfoBuffer.AddLocal(target.category, target.id, lastRotation);
                }
            }
            else
            {
                if (transform.position != lastPosition)
                {
                    lastPosition = transform.position;
                    InfoBuffer.Add(target.category, target.id, lastPosition);
                }
                if (transform.rotation != lastRotation)
                {
                    lastRotation = transform.rotation;
                    InfoBuffer.Add(target.category, target.id, lastRotation);
                }
            }
        }
    }

}

