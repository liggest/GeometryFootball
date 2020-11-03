using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarCollision : MonoBehaviour
{
    public float shootForce = 100;
    Transform center;

    private void Start()
    {
        center = transform.parent;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            Transform target = collision.transform;
            Rigidbody rig = target.GetComponent<Rigidbody>();
            Vector3 direction = target.position - center.position;
            direction.y = 0;
            direction = direction.normalized;
            rig.AddForce(direction * shootForce, ForceMode.Impulse);
        }      
    }
}
