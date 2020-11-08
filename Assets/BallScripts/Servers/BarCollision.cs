﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallScripts.GameLogics;

namespace BallScripts.Servers
{
    public class BarCollision : MonoBehaviour
    {
        public float shootForce = 10;
        Transform center;

        private void Start()
        {
            if (!center)
            {
                InitCenter();
            }
        }

        public void InitCenter()
        {
            center = transform.parent;
        }

        private void OnCollisionEnter(Collision collision)
        {
            bool dealed = true;
            Transform otherPlayer;
            Transform thisPlayer;
            switch (collision.collider.tag)
            {
                case "Ball":
                    Bounce(collision.transform, 1.0f);
                    break;
                case "Bar":
                    otherPlayer = collision.transform.parent.parent;
                    thisPlayer = center.parent;
                    Bounce(otherPlayer, 0.7f);
                    Bounce(thisPlayer, GetDirection(otherPlayer, thisPlayer), 0.7f);
                    break;
                case "Player":
                    otherPlayer = collision.transform.parent.parent;
                    Bounce(otherPlayer, 0.7f);
                    break;
                default:
                    dealed = false;
                    break;
            }
            if (!dealed && collision.gameObject.TryGetComponent(out BaseStageObject obj)) 
            {
                switch (obj.category)
                {
                    case StageObjectCategory.Dynamic:
                        Bounce(collision.transform, 1.0f);
                        break;
                    case StageObjectCategory.Static:
                        thisPlayer = center.parent;
                        Bounce(thisPlayer, GetDirection(collision.transform, thisPlayer), 0.4f);
                        break;
                    case StageObjectCategory.Goal:
                    case StageObjectCategory.Other:
                    default:
                        break;
                }
            }
        }

        void Bounce(Transform target, float bounceFactor)
        {
            Bounce(target, GetDirection(target), bounceFactor);
        }

        void Bounce(Transform target, Vector3 direction, float bounceFactor)
        {
            if (target.TryGetComponent(out Rigidbody rig))
            {
                rig.AddForce(direction * shootForce * bounceFactor, ForceMode.Impulse);
            }
        }

        Vector3 GetDirection(Transform target)
        {
            return GetDirection(center, target);
        }

        Vector3 GetDirection(Transform origin, Transform target)
        {
            Vector3 direction = target.position - origin.position;
            direction.y = 0;
            direction = direction.normalized;
            return direction;
        }
    }
}
