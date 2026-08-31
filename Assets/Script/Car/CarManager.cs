using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Rendering;

public class CarManager : MonoBehaviour
{
    public WheelCollider FLWheel;
    public WheelCollider FRWheel;
    public WheelCollider RLWheel;
    public WheelCollider RRWheel;



    public Rigidbody RB;

    private CarStat stat;

 

    //단순 상태값.
    public float Speed => RB.linearVelocity.magnitude * 3.6f;




    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        stat = GetComponent<CarStat>();
        
    }

    public void Accelerate(int direction)
    {
        RLWheel.motorTorque = direction * stat.Acceleration;
        RRWheel.motorTorque = direction * stat.Acceleration;

    }
    public void Steer(int steer)
    {

    }
 
    public void Brake()
    {
        RLWheel.motorTorque = 0f;
        RRWheel.motorTorque = 0f;

        FLWheel.brakeTorque = stat.Braking;
        FRWheel.brakeTorque = stat.Braking;
        RLWheel.brakeTorque = stat.Braking;
        RLWheel.brakeTorque = stat.Braking;
    }



}
