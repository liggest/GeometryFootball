using System.Collections;
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
            center = transform.parent;
        }
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.TryGetComponent(out BaseStageObject obj))
            {
                switch (obj.category)
                {
                    case StageObjectCategory.Player:
                        Bounce(collision.transform, 1.5f);
                        break;
                    case StageObjectCategory.Ball:
                    case StageObjectCategory.Dynamic:
                        Bounce(collision.transform, 1.0f);
                        break;
                    case StageObjectCategory.Static:
                        Bounce(center, 0.4f);
                        break;
                    case StageObjectCategory.Goal:
                    case StageObjectCategory.Other:
                    default:
                        break;
                }
            }

            if (collision.collider.CompareTag("Ball"))
            {

            }
        }

        void Bounce(Transform target, float bounceFactor)
        {
            if (target.TryGetComponent(out Rigidbody rig))
            {
                Vector3 direction = target.position - center.position;
                direction.y = 0;
                direction = direction.normalized;
                rig.AddForce(direction * shootForce * bounceFactor, ForceMode.Impulse);
            }
        }
    }
}
