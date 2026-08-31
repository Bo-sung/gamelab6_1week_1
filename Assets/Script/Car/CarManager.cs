using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Rendering;

public class CarManager : MonoBehaviour
{

    public Rigidbody RB;

    //증강에 영향받는 상태값.
    public float CarMaxSpeed;
    public float CarAccelerationLevel;
    public float CarBrakeLevel;
    public float SteeringLevel;
    public float Mass;

    //단순 상태값.
    public float Speed => RB.linearVelocity.magnitude * 3.6f;

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    public void Accelerate(int direction)
    {
        if(RB != null) 
        {
            if (direction == 1)
            {
                RB.AddForce(transform.forward * CarAccelerationLevel);
            }
            else if (direction == -1) 
            {
                RB.AddForce(transform.forward * -CarAccelerationLevel);
            }
        }
    }
    public void Steer(int steer)
    {
        if (steer == -1)
        {
            RB.AddForce(transform.right * -CarAccelerationLevel);
            RB.AddRelativeTorque(Vector3.up * 1);

        }
        else if (steer == 1)
        {
            RB.AddForce(transform.right * CarAccelerationLevel);
            RB.AddRelativeTorque(Vector3.up * -1);
        }
    }
 
    public void Brake()
    {
        RB.linearVelocity = RB.linearVelocity / CarBrakeLevel;
    }



}
