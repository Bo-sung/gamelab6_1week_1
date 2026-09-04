using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.GraphicsBuffer;
using Cursor = UnityEngine.Cursor;
using Debug = UnityEngine.Debug;

public class ArrowController : MonoBehaviour
{
    [Header("Default")]
    [SerializeField] float sensitivity = 3f;
    float currentSensitivity = 3f;
    [SerializeField] float minPitch = -20f, maxPitch = 70f;
    [SerializeField] float speed = 2f;
    [SerializeField] float minimumSpeed = 0f;
    [SerializeField] float acc_per_tick = 1f;
    [SerializeField] bool autoAcc = false;
    [SerializeField] Transform arrowModel;
    [SerializeField]
    float curAcceleration = 0f;

    [Header("Dash")]
    [SerializeField] float dashSpeed = 4f;
    // 대시 지속시간
    [SerializeField] float dashTime = 1f;
    float remainDashTime = 0f;
    // 대시 발동 후 다음 쿨타임
    [SerializeField] float dashCooldown = 2f;
    float remainDashCooldown = 0f;
    [SerializeField] float dashTimeScale = 0f;
    float defaultTimeScale = 1f;
    [SerializeField] float dashTimeSensitivity = 0f;
    [SerializeField]
    float chargeTime = 2f;
    public float ChargeTime => chargeTime;
    [SerializeField] float chargeJitter = 0.5f;

    [SerializeField]
    Transform rayOrigin;
    [Header("Monitor")]
    [SerializeField] float dashChargingTime = 0f;
    [SerializeField] float curSpeed = 0f;

    public System.Action OnDashStart;
    public System.Action<DashState> OnDashStateChanged;
    public System.Action OnHitEnemy;
    public System.Action OnHitGround;

    private bool isFlyable = true;

    public bool IsFlyable => isFlyable;

    float yaw, pitch = 28f;

    private float chargeTimer = 0f;
    private Rigidbody rb;

    private Vector3 lockTarget;

    public enum DashState
    {
        None,
        Ready,
        Dash,
        Charging,
        Dashing,
        Cooldown
    }

    public DashState dashState = DashState.None;

    void Start()
    {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleAcceleration();
        HandleDashLogic();
    }


    private void HandleAcceleration()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            curAcceleration += acc_per_tick * Time.deltaTime;
        }
        else
        {
            curAcceleration -= acc_per_tick * Time.deltaTime;
        }
        curAcceleration = Mathf.Clamp01(curAcceleration);
    }
    private void HandleDashLogic()
    {
        // 대시 상태 전환
        if (dashState == DashState.Dashing && remainDashTime <= 0)
            ChangeDashState(DashState.Cooldown);
        else if (dashState == DashState.Cooldown && remainDashCooldown <= 0)
            ChangeDashState(DashState.None);
        else if (Input.GetKeyUp(KeyCode.Mouse1) && dashState == DashState.Charging)
            ChangeDashState(DashState.Dash);
        else if (Input.GetKeyDown(KeyCode.Mouse1) && dashState == DashState.None)
            ChangeDashState(DashState.Ready);
        else if (dashState == DashState.Charging && chargeTimer >= chargeTime)
            ChangeDashState(DashState.Dash);

        ProcessDash();

        // 타이머 감소
        remainDashTime -= Time.deltaTime;
        remainDashCooldown -= Time.deltaTime;
    }

    private void ChangeDashState(DashState state)
    {
        dashState = state;
        OnDashStateChanged?.Invoke(dashState);
    }

    private void ProcessDash()
    {
        switch (dashState)
        {
            // 대시 사용
            case DashState.Dash:
                // 대시 실행 후 타임스케일과 민감도 원래대로 복구
                DoDash();
                break;
            // 차징 시작
            case DashState.Ready:
                // 상태 전환
                ChangeDashState(DashState.Charging);
                dashChargingTime = 0;
                chargeTimer = 0f;
                break;
            // 차징 중
            case DashState.Charging:
                // 차징중 시간 느려짐 효과 + 민감도 증가(시간이 늘어짐에 따라. 유저 입력에 보정을 주기 위해)
                chargeTimer += Time.unscaledDeltaTime;
                dashChargingTime += Time.deltaTime;
                Time.timeScale = Mathf.Lerp(defaultTimeScale, dashTimeScale, dashChargingTime);
                currentSensitivity = Mathf.Lerp(sensitivity, dashTimeSensitivity, dashChargingTime);
                break;
            // 대시 중
            case DashState.Dashing:
                break;
            // 쿨타임 중
            case DashState.Cooldown:
                lockTarget = Vector3.zero;
                break;
        }
    }

    private void DoDash()
    {
        ChangeDashState(DashState.Dashing);
        remainDashTime = dashTime;
        remainDashCooldown = dashCooldown;

        if (lockTarget != null && lockTarget != Vector3.zero)
        {
            Vector3 direction = transform.position - lockTarget;

            ReCalcInput(direction.normalized);

            // 입력 받은 값 토대로 회전 처리
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        Time.timeScale = defaultTimeScale;
        currentSensitivity = sensitivity;
        OnDashStart?.Invoke();

        //타이머 초기화
        chargeTimer = 0f;

        UnityEngine.Debug.Log("Dash executed!");
    }

    void LateUpdate()
    {
        // 대시 등, 상태 처리
        float moveSpeed = 0;
        switch (dashState)
        {
            case DashState.Dashing:
                moveSpeed = dashSpeed * dashChargingTime;
                break;
            case DashState.Charging:
                {
                    // 대시만 아니면 회전 가능
                    // 마우스 입력 처리
                    yaw += Input.GetAxis("Mouse X") * currentSensitivity;
                    pitch -= Input.GetAxis("Mouse Y") * currentSensitivity;
                    pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

                    // 입력 받은 값 토대로 회전 처리
                    transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

                    // 만약 자동가속이면 스킵
                    if (autoAcc)
                    {
                        moveSpeed = speed;
                        break;
                    }
                    // 자동 가속 아니면 스무스하게 정지
                    moveSpeed = Mathf.Lerp(speed, minimumSpeed, curAcceleration);
                    Debug.DrawLine(rayOrigin.position, rayOrigin.position + (rayOrigin.position - transform.position).normalized * dashSpeed * dashChargingTime, Color.aquamarine);
                    var hits = Physics.RaycastAll(rayOrigin.position, (rayOrigin.position - transform.position).normalized, dashSpeed * dashChargingTime);
                    if (hits != null)
                    {
                        // 거리(distance)가 가까운 순서대로 정렬
                        RaycastHit[] sortedHits = hits.OrderBy(h => h.distance).ToArray();

                        foreach (var hit in sortedHits)
                        {
                            if (hit.transform.CompareTag("Enemy"))
                            {
                                var enemy = hit.transform.GetComponent<EnemyBase>();
                                if (enemy != null)
                                {
                                    lockTarget = hit.transform.position;
                                    Debug.Log($"적 인식, point {lockTarget}");
                                }
                            }
                        }
                    }
                }
                break;
            case DashState.Cooldown:
            case DashState.None:
                // 대시만 아니면 회전 가능
                // 마우스 입력 처리
                yaw += Input.GetAxis("Mouse X") * currentSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * currentSensitivity;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

                // 입력 받은 값 토대로 회전 처리
                transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

                // 만약 자동가속이면 스킵
                if (autoAcc)
                {
                    moveSpeed = speed;
                    break;
                }
                // 자동 가속 아니면 스무스하게 정지
                moveSpeed = Mathf.Lerp(speed, minimumSpeed, curAcceleration);
                break;
            default:
                UnityEngine.Debug.Log("이거 이상한데 여기 왜탐?");
                break;
        }


        // 이동 시작
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

        curSpeed = moveSpeed;

        //바닥 아래로 이동하는 현상 방지
        if (transform.position.y < 0.4f)
        {
            transform.position = new Vector3(transform.position.x, 0.4f, transform.position.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Body") || other.CompareTag("Head") || other.CompareTag("Enemy"))
        {
            OnHitEnemy?.Invoke();
            if (dashState == DashState.Charging)
            {
                ChangeDashState(DashState.Dash);
            }
        }
        else if (other.CompareTag("Ground"))
        {
            MakeStatic();
            Debug.Log("땅에 닿음");
        }
    }

    private void MakeStatic()
    {
        isFlyable = false;
        rb.isKinematic = true;
        GetComponentInChildren<CapsuleCollider>().isTrigger = false;
        this.enabled = false;
    }

    private void MakeDynamic()
    {
        isFlyable = true;
        rb.isKinematic = false;
        GetComponentInChildren<CapsuleCollider>().isTrigger = true;
        this.enabled = true;
    }

    public void ReSetYawPitch()
    {
        ReCalcInput(transform.forward);
    }

    private void ReCalcInput(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.000001f)
            return;

        float horizontalMagnitude =
            Mathf.Sqrt
            (
                direction.x * direction.x +
                direction.z * direction.z
            );

        float targetPitch = Mathf.Atan2(-direction.y, horizontalMagnitude) * Mathf.Rad2Deg;

        // 수직에 너무 가까우면 yaw는 기존 값 유지
        if (horizontalMagnitude > 0.0001f)
        {
            float targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            this.yaw += Mathf.DeltaAngle(this.yaw, targetYaw);
        }

        this.pitch = Mathf.Clamp
            (
            targetPitch,
            minPitch,
            maxPitch
        );
    }
}
