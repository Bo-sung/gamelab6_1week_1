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
    public float Speed => RB.linearVelocity.magnitude;

    public float SpeedKmh => Speed * 3.6f;


    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        stat = GetComponent<CarStat>();
        
    }

    public void Accelerate(float acc)
    {

        RLWheel.motorTorque = acc * stat.Acceleration*100 * (1000 / stat.Weight);
        RRWheel.motorTorque = acc * stat.Acceleration*100 * (1000 / stat.Weight);



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

    //매 프레임 상태값에 따라 조정
    public void AdjustStauts()
    {
        //제한속도 조정
        if(Speed > stat.Speed)
        {
            RLWheel.motorTorque = 0f;
            RRWheel.motorTorque = 0f;
        }

 
    }

    public void RefreshStat()
    {

        ChangeStiffness(FLWheel, stat.Cornering * 0.3f);
        ChangeStiffness(FRWheel, stat.Cornering * 0.3f);
        ChangeStiffness(RLWheel, stat.Cornering * 0.3f);
        ChangeStiffness(RRWheel, stat.Cornering * 0.3f);



    }

    private void ChangeStiffness(WheelCollider wheel, float stiffness)
    {
        var friction = wheel.sidewaysFriction;
        friction.stiffness = stiffness;
        wheel.sidewaysFriction = friction;
    }

}
