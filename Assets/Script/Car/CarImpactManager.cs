using System;
using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CarImpactManager : MonoBehaviour
{
    [SerializeField]
    private float ImpactionPower = 1000f;
    public Action<string> OnWallCollsion;

    private Rigidbody rb;
    private CarStat stat;

    private float collisionTimer;

    public void Initialize(CarStat stat)
    {
        this.stat = stat;
        rb= GetComponent<Rigidbody>();
        collisionTimer = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.CompareTag("Wall"))
        {
            if (collisionTimer < 0.3f) return;
            OnWallCollsion?.Invoke("Wall");
            Debug.Log("Wall Collision");
            //충격량 구현.
            var contact = collision.contacts[0];

            var reflected = Vector3.Reflect(rb.linearVelocity, contact.normal);

            rb.AddForce(contact.normal * collision.relativeVelocity.magnitude * ImpactionPower *(1000/stat.Weight), ForceMode.Impulse);
            collisionTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        collisionTimer += Time.fixedDeltaTime;
       
    }

}