using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
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
    [SerializeField] float curAcceleration = 0f;

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
    [SerializeField] float chargeTime = 2f;
    public float ChargeTime => chargeTime;
    [SerializeField] float chargeJitter = 0.5f;

    [SerializeField] Transform rayOrigin;
    [Header("Monitor")]
    [SerializeField] float dashChargingTime = 0f;
    [SerializeField] float curSpeed = 0f;

    [Header("Return")]
    [SerializeField] private NavMeshAgent returningAgent;

    [SerializeField] private float navSearchDistance = 5f;
    [SerializeField] private float returnRefreshTime = 0.1f;
    [SerializeField] private float returnArriveDistance = 0.5f;

    [SerializeField] LogRoller effect;

    private Transform returnTarget;
    private float returnRefreshTimer;

    private Vector3 lastReturnDestination;

    public bool IsReturning { get; private set; }

    public System.Action OnReturnComplete;

    public System.Action OnDashStart;
    public System.Action<DashState> OnDashStateChanged;
    public System.Action OnHitEnemy;
    public System.Action OnHitGround;

    float yaw, pitch = 28f;

    private float chargeTimer = 0f;
    private Rigidbody rb;

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


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        returningAgent = GetComponent<NavMeshAgent>();

    }
    void Start()
    {
        ReSetYawPitch();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (IsReturning)
        {
            UpdateReturn();
            return;
        }

        HandleAcceleration();
        HandleDashLogic();
    }

    void OnEnable()
    {
        MakeDynamic();
        ReSetYawPitch();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
                break;
        }
    }

    private void DoDash()
    {
        ChangeDashState(DashState.Dashing);
        remainDashTime = dashTime;
        remainDashCooldown = dashCooldown;

        Time.timeScale = defaultTimeScale;
        currentSensitivity = sensitivity;
        OnDashStart?.Invoke();

        //타이머 초기화
        chargeTimer = 0f;

        UnityEngine.Debug.Log("Dash executed!");
    }

    void LateUpdate()
    {
        if(IsReturning) 
            return;
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
                Debug.Log($"post CalcInput = {transform.rotation}");

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

    void OnTriggerEnter(Collider other)
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
            GetComponentInChildren<CapsuleCollider>().isTrigger = false;
            OnHitGround?.Invoke();
            if (!IsReturning)
                this.enabled = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            GetComponentInChildren<CapsuleCollider>().isTrigger = false;
            OnHitGround?.Invoke();
            if(!IsReturning)
                this.enabled = false;
        }
    }

    public void MakeStatic()
    {
        GetComponentInChildren<CapsuleCollider>().isTrigger = false;
        OnHitGround?.Invoke();
        if (!IsReturning)
            this.enabled = false;
    }

    public void MakeDynamic()
    {
        GetComponentInChildren<CapsuleCollider>().isTrigger = true;
    }

    public void ReSetYawPitch()
    {
        var result = CommonUtils.ReCalcInput(transform.forward);

        pitch = result.pitch;
        yaw = result.yaw;
    }

    public bool StartReturn(Transform target)
    {
        if (target == null)
            return false;

        if (returningAgent == null)
        {
            Debug.LogError("NavMeshAgent가 없습니다.");
            return false;
        }

        // 현재 화살 근처에서 이 Agent가 사용할 수 있는 NavMesh 탐색
        if (!TryGetNavMeshPosition(transform.position, out Vector3 startPosition))
        {
            Debug.Log("귀환 시작 가능한 NavMesh가 없음");
            return false;
        }

        returnTarget = target;
        IsReturning = true;
        effect.enabled = true;

        // 바닥 충돌 후 ArrowController가 꺼진 상태일 수 있음
        enabled = true;

        // 기존 비행 / 물리 영향 제거
        if (rb != null)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
        rb.velocity = Vector3.zero;
#endif
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        CapsuleCollider col = GetComponentInChildren<CapsuleCollider>();

        if (col != null)
            col.isTrigger = true;

        // Agent가 기존 위치에서 켜지는 것을 방지
        returningAgent.enabled = false;

        // 화살 자체를 NavMesh 위로 이동
        transform.position = startPosition;

        returningAgent.enabled = true;

        if (!returningAgent.isOnNavMesh)
        {
            Debug.LogWarning($"NavMeshAgent 진입 실패 : {startPosition}");

            returningAgent.enabled = false;
            IsReturning = false;
            effect.enabled = false;
            returnTarget = null;

            return false;
        }

        // 내부 Agent 위치도 명확하게 맞춤
        returningAgent.Warp(startPosition);

        returningAgent.isStopped = false;
        returningAgent.ResetPath();

        // 바로 목적지 계산
        returnRefreshTimer = 0f;

        if (!RefreshReturnDestination())
        {
            CancelReturn();
            return false;
        }

        return true;
    }

    private bool TryGetNavMeshPosition(Vector3 worldPosition, out Vector3 navPosition)
    {
        NavMeshQueryFilter filter = new NavMeshQueryFilter
        {
            agentTypeID = returningAgent.agentTypeID,
            areaMask = returningAgent.areaMask
        };

        if (NavMesh.SamplePosition(worldPosition, out NavMeshHit hit, navSearchDistance, filter))
        {
            navPosition = hit.position;
            return true;
        } else if (NavMesh.SamplePosition(worldPosition, out NavMeshHit hit2, navSearchDistance * 10, filter))
        {
            navPosition = hit2.position;
            return true;
        } else if (NavMesh.SamplePosition(worldPosition, out NavMeshHit hit3, navSearchDistance * 40, filter))
        {
            navPosition = hit3.position;
            return true;
        }

        navPosition = default;
        return false;
    }

    private void UpdateReturn()
    {
        if (!IsReturning)
            return;

        if (returnTarget == null)
        {
            CancelReturn();
            return;
        }

        if (!returningAgent.enabled || !returningAgent.isOnNavMesh)
        {
            CancelReturn();
            return;
        }

        returnRefreshTimer -= Time.deltaTime;

        if (returnRefreshTimer <= 0f)
        {
            RefreshReturnDestination();

            returnRefreshTimer = returnRefreshTime;
        }

        // 경로 계산 중에는 도착 판정하지 않음
        if (returningAgent.pathPending)
            return;

        Vector3 delta = lastReturnDestination - returningAgent.transform.position;

        delta.y = 0f;

        if (delta.sqrMagnitude <= returnArriveDistance * returnArriveDistance)
        {
            CompleteReturn();
        }

        Debug.Log($"NavDestination = {returningAgent.destination}");
    }

    private bool RefreshReturnDestination()
    {
        if (returnTarget == null)
            return false;

        // 플레이어 Transform 자체가 NavMesh 위에 있을 필요 없음.
        // 플레이어 근처의 NavMesh 점을 목적지로 사용.
        if (!TryGetNavMeshPosition(returnTarget.position, out Vector3 destination))
        {
            Debug.LogWarning($"귀환 목적지 근처 NavMesh 없음 : " + $"{returnTarget.position}");

            return false;
        }

        lastReturnDestination = destination;

        return returningAgent.SetDestination(destination);
    }

    private void CompleteReturn()
    {
        if (!IsReturning)
            return;

        IsReturning = false;
        effect.enabled = false;
        returnTarget = null;

        if (returningAgent != null && returningAgent.enabled)
        {
            if (returningAgent.isOnNavMesh)
            {
                returningAgent.isStopped = true;
                returningAgent.ResetPath();
            }

            returningAgent.enabled = false;
        }

        OnReturnComplete?.Invoke();
    }

    public void CancelReturn()
    {
        IsReturning = false;
        effect.enabled = false;
        returnTarget = null;

        if (returningAgent == null)
            return;

        if (returningAgent.enabled)
        {
            if (returningAgent.isOnNavMesh)
            {
                returningAgent.isStopped = true;
                returningAgent.ResetPath();
            }

            returningAgent.enabled = false;
        }
    }

    public void FinishReturn()
    {
        if (!IsReturning)
            return;

        CompleteReturn();
    }
}
