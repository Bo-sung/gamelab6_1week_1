using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CarManager : MonoBehaviour
{
    private const float CorneringToStiffnessMultiplier = 0.2f;

    public WheelCollider FLWheel;
    public WheelCollider FRWheel;
    public WheelCollider RLWheel;
    public WheelCollider RRWheel;

    public Transform FLWheelMesh;
    public Transform FRWheelMesh;
    public Transform RLWheelMesh;
    public Transform RRWheelMesh;

    
    [SerializeField]
    private float MaxSteerAngle = 5;
    [SerializeField]
    private float MortorTorqueMultiplier = 50f;
    [SerializeField]
    private float BrakeTorqueMultiplier = 100f;
    [SerializeField]
    private float MaxSteerTime = 1f;
    [SerializeField]
    private float SteerRerocateLevel = 0.05f;
    [SerializeField]
    private float DownForceLevel = 10f;


    private Rigidbody RB;

    private CarStat stat;



    //단순 상태값.
    public float Speed => RB.linearVelocity.magnitude;

    public float SpeedKmh => Speed * 3.6f;

    private float steerAngle = 0f;

    private bool steerControlOnFrame;



    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        stat = GetComponent<CarStat>();
        
    }

    private void Update()
    {
        UpdateWheelVisual(FLWheelMesh, FLWheel);
        UpdateWheelVisual(FRWheelMesh, FRWheel);
        UpdateWheelVisual(RLWheelMesh, RLWheel);
        UpdateWheelVisual(RRWheelMesh, RRWheel);
    }

    public void Accelerate(float acc)
    {

        RLWheel.motorTorque = acc * stat.Acceleration* MortorTorqueMultiplier * (1000 / stat.Weight);
        RRWheel.motorTorque = acc * stat.Acceleration* MortorTorqueMultiplier * (1000 / stat.Weight);



    }
    public void Steer(float steer)
    {
        if(steer != 0f) steerControlOnFrame = true;

        var steerDelta = Mathf.Clamp(steer * Time.deltaTime / MaxSteerTime,-1,1);

        if (steerAngle == 0f) steerAngle += steerDelta;

        else if(steerDelta * steerAngle > 0)//같은 각일때
            steerAngle = Mathf.Clamp(steerAngle + steerDelta, -1, 1);
    
       
            
        else if(steerDelta * steerAngle < 0)//다른 각일때
            steerAngle = Mathf.Clamp(steerAngle + (steerDelta*2), -1, 1);
        
        
        

        FLWheel.steerAngle = steerAngle * MaxSteerAngle;
        FRWheel.steerAngle = steerAngle * MaxSteerAngle;

      

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

        RLWheel.brakeTorque = stat.Braking * BrakeTorqueMultiplier;
        RLWheel.brakeTorque = stat.Braking * BrakeTorqueMultiplier;
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
        //다운포스
        RB.AddForce(-transform.up * DownForceLevel,ForceMode.Force);

        if(!steerControlOnFrame)
        {
            if(steerAngle  > 0)
            {
                var absSteer = Mathf.Clamp01(steerAngle - SteerRerocateLevel);
                steerAngle = absSteer;
            }
            else if (steerAngle < 0)
            {
                var absSteer = Mathf.Clamp(steerAngle + SteerRerocateLevel,-1,0);
                steerAngle = absSteer;
            }

            Debug.Log(steerAngle);
        }
        steerControlOnFrame = false;

       
 
    }

    public void RefreshStat()
    {

        ChangeStiffness(FLWheel, stat.Cornering * CorneringToStiffnessMultiplier);
        ChangeStiffness(FRWheel, stat.Cornering * CorneringToStiffnessMultiplier);
        ChangeStiffness(RLWheel, stat.Cornering * CorneringToStiffnessMultiplier);
        ChangeStiffness(RRWheel, stat.Cornering * CorneringToStiffnessMultiplier);

    }

    private void ChangeStiffness(WheelCollider wheel, float stiffness)
    {
        var friction = wheel.sidewaysFriction;
        friction.stiffness = stiffness;
        wheel.sidewaysFriction = friction;
    }


    private void UpdateWheelVisual(Transform trans, WheelCollider wheelCol)
    {
        Vector3 UpdatePos;
        Quaternion UpdateRot;

        //휠 운동 연산 결과를 월드 좌표로 변환
        wheelCol.GetWorldPose(out UpdatePos, out UpdateRot);

        trans.position = UpdatePos;
        trans.rotation = UpdateRot;
    }
}
