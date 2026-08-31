using System.ComponentModel.Design;
using Unity.VisualScripting;
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

    public void Accelerate(float direction)
    {
        RLWheel.motorTorque = direction * stat.Acceleration*100;
        RRWheel.motorTorque = direction * stat.Acceleration*100;

        

    }
    public void Steer(float steer)
    {
        FLWheel.steerAngle = steer * stat.Cornering;
        FRWheel.steerAngle = steer * stat.Cornering;
    }
 
    public void Brake(bool brake)
    {
        if (!brake)
        {
            FLWheel.brakeTorque = 0;
            FRWheel.brakeTorque = 0;
            RLWheel.brakeTorque = 0;
            RLWheel.brakeTorque = 0;
            return;
        }

      
        RLWheel.motorTorque = 0f;
        RRWheel.motorTorque = 0f;

        FLWheel.brakeTorque = stat.Braking * 100;
        FRWheel.brakeTorque = stat.Braking * 100;
        RLWheel.brakeTorque = stat.Braking * 100;
        RLWheel.brakeTorque = stat.Braking * 100;
    }



}
