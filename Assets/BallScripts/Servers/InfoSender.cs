using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;

namespace BallScripts.Servers
{
    public class InfoSender : MonoBehaviour
    {
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

