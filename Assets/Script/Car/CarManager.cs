using System.Collections;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class CarManager : MonoBehaviour
{
    private const float CorneringToStiffnessMultiplier = 0.15f;

    public WheelCollider FLWheel;
    public WheelCollider FRWheel;
    public WheelCollider RLWheel;
    public WheelCollider RRWheel;

    public Transform FLWheelMesh;
    public Transform FRWheelMesh;
    public Transform RLWheelMesh;
    public Transform RRWheelMesh;

    [SerializeField]
    private CarImpactManager carImpactManager;
    
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



    private Rigidbody rb;

    private CarStat stat;



    //단순 상태값.
    public float Speed => rb.linearVelocity.magnitude;

    public float SpeedKmh => Speed * 3.6f;

    private float steerAngle = 0f;

    private bool steerControlOnFrame;
    private bool downForceFlag;

    private float collisionMortorFlag;

    //코루틴 상태값
    private Coroutine collsionCoroutine;


    public void Initialize(CarStat stat)
    {
        rb = GetComponent<Rigidbody>();
        if(stat == null) this.stat = GetComponent<CarStat>();
        else this.stat = stat;

        carImpactManager.Initialize(this.stat);
        carImpactManager.OnWallCollsion += OnWallCollision;
        collisionMortorFlag = 1f;
    }

    private void Update()
    {
        UpdateWheelVisual(FLWheelMesh, FLWheel);
        UpdateWheelVisual(FRWheelMesh, FRWheel);
        UpdateWheelVisual(RLWheelMesh, RLWheel);
        UpdateWheelVisual(RRWheelMesh, RRWheel);
    }

    private void OnDestroy()
    {
        carImpactManager.OnWallCollsion -= OnWallCollision;
    }
    public void Accelerate(float acc)
    {

        RLWheel.motorTorque = acc * stat.Acceleration* MortorTorqueMultiplier * (1000 / stat.Weight) * collisionMortorFlag;
        RRWheel.motorTorque = acc * stat.Acceleration* MortorTorqueMultiplier * (1000 / stat.Weight) * collisionMortorFlag;



    }
    public void Steer(float steer)
    {
        if(steer != 0f) steerControlOnFrame = true;

        var steerDelta = Mathf.Clamp(steer * Time.deltaTime / MaxSteerTime,-1,1);


        steerAngle = Mathf.Clamp(steerAngle + steerDelta, -1, 1);

        FLWheel.steerAngle = steerAngle * MaxSteerAngle / Mathf.Sqrt(Speed);
        FRWheel.steerAngle = steerAngle * MaxSteerAngle / Mathf.Sqrt(Speed);



        
      

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
        // 튕겨 나가는 상황을 위한 각속도 조정;
        if(rb.angularVelocity.magnitude > 2f)
        {
            rb.angularVelocity = Vector3.zero;
        }

        
        //다운포스
        if(downForceFlag)   rb.AddForce(-transform.up * DownForceLevel,ForceMode.Force);

        //핸들 미조작시 정렬
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

         
        }
        //Stiffness를 매 프레임 stat 값 및 충돌 상태에 따라 조정한다.
        ChangeStiffness(FLWheel,0.5f + stat.Cornering * CorneringToStiffnessMultiplier * collisionMortorFlag);
        ChangeStiffness(FRWheel, 0.5f + stat.Cornering * CorneringToStiffnessMultiplier * collisionMortorFlag);
        ChangeStiffness(RLWheel, 0.5f + stat.Cornering * CorneringToStiffnessMultiplier * collisionMortorFlag);
        ChangeStiffness(RRWheel,0.5f + stat.Cornering * CorneringToStiffnessMultiplier * collisionMortorFlag);
        
        //플래그 초기화
        steerControlOnFrame = false;

       
 
    }

    private void OnWallCollision(string wall)
    {
        if(collsionCoroutine != null)
        {
            StopCoroutine(collsionCoroutine);
        }

        collsionCoroutine = StartCoroutine(CollisionAdjust());

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


    IEnumerator CollisionAdjust()
    {

        downForceFlag = false;
        collisionMortorFlag = 0f;

        yield return new WaitForSeconds(0.5f);

       
        downForceFlag = true;
        collisionMortorFlag = 1f;

    }
}
