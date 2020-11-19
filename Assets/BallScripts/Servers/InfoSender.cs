using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;

namespace BallScripts.Servers
{
    [Flags]
    public enum SendFlag
    {
        Position = 1,
        Rotation = 2,
        LocalPosition = 4,
        LocalRotation = 8,
        LocalScale = 16
    }

    public class InfoSender : MonoBehaviour
    {
        public SendFlag sendFlags = SendFlag.Position | SendFlag.Rotation;
        //public bool sendLocal = false;
        //开启后将转而发送local位置、旋转
        //local位置、旋转 和 全局位置、旋转，逻辑上只需要同步其一

        Vector3 lastPosition;
        Quaternion lastRotation;
        Vector3 lastScale;
        BaseStageObject target;

        // Start is called before the first frame update
        void Start()
        {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            lastScale = transform.localScale;
            target = GetComponent<BaseStageObject>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Send(sendFlags);
        }

        public void Send(SendFlag flags)
        {
            if ((flags & SendFlag.Position) != 0)
            {
                if (transform.position != lastPosition)
                {
                    lastPosition = transform.position;
                    InfoBuffer.Add(target.category, target.id, lastPosition);
                }
            }
            else if ((flags & SendFlag.LocalPosition) != 0)
            {
                if (transform.localPosition != lastPosition)
                {
                    lastPosition = transform.localPosition;
                    InfoBuffer.AddLocal(target.category, target.id, lastPosition);
                }
            }
            if ((flags & SendFlag.Rotation) != 0)
            {
                if (transform.rotation != lastRotation)
                {
                    lastRotation = transform.rotation;
                    InfoBuffer.Add(target.category, target.id, lastRotation);
                }
            }
            else if ((flags & SendFlag.LocalRotation) != 0)
            {
                if (transform.localRotation != lastRotation)
                {
                    lastRotation = transform.localRotation;
                    InfoBuffer.AddLocal(target.category, target.id, lastRotation);
                }
            }
            if ((flags & SendFlag.LocalScale) != 0)
            {
                if (transform.localScale != lastScale)
                {
                    lastScale = transform.localScale;
                    InfoBuffer.AddLocalScale(target.category, target.id, lastScale);
                }
            }
        }
    }

}

