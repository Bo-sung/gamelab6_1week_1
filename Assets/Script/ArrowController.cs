using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

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
    [SerializeField] float chargeTime = 2f;
    [SerializeField]float chargeJitter = 0.5f;

    [Header("Monitor")]
    [SerializeField] float dashChargingTime = 0f;
    [SerializeField] float curSpeed = 0f;

    public System.Action OnDashStart;
    public System.Action<DashState> OnDashStateChanged;
    public System.Action OnHitEnemy;

    float yaw, pitch = 28f;

    private float chargeTimer = 0f;
    private Coroutine arrowJitterCoroutine;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleAcceleration();
        HandleDashLogic();

        
    }

    private void FixedUpdate()
    {

        //바닥 아래로 이동하는 현상 방지
        if (transform.position.y < 0.5f)
        {
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        }
        
    }

    private void HandleAcceleration()
    {
        if (Input.GetKey(KeyCode.W))
        {
            curAcceleration += acc_per_tick * Time.deltaTime;
        }
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
        //UnityEngine.Debug.Log($"DashState: {dashState}, remainDashTime: {remainDashTime}, remainDashCooldown: {remainDashCooldown}");
    }

    private void ChangeDashState(DashState state)
    {
        //UnityEngine.Debug.Log($"DashState Changed From {dashState} To {state}");
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
                if(arrowJitterCoroutine != null) StopCoroutine(arrowJitterCoroutine);
                arrowJitterCoroutine = StartCoroutine(ArrowJitter());


                break;
            // 차징 중
            case DashState.Charging:
                // 차징중 시간 느려짐 효과 + 민감도 증가(시간이 늘어짐에 따라. 유저 입력에 보정을 주기 위해)
                chargeTimer += Time.deltaTime;
                dashChargingTime += Time.deltaTime;
                Time.timeScale = Mathf.Lerp(defaultTimeScale, dashTimeScale, dashChargingTime);
                currentSensitivity = Mathf.Lerp(sensitivity, dashTimeSensitivity, dashChargingTime);
                break;
            // 대시 중
            case DashState.Dashing:
                break;
            // 쿨타임 중
            case DashState.Cooldown:
                // Handle cooldown state logic
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

        //흔들림 제거
        if(arrowJitterCoroutine != null)
        {
            StopCoroutine(arrowJitterCoroutine);
            arrowJitterCoroutine = null;
            arrowModel.transform.localRotation = Quaternion.Euler(90, 0, 0); // Reset rotation
        }
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
            case DashState.Cooldown:
            case DashState.None:
            case DashState.Charging:
                // 대시만 아니면 회전 가능
                // 마우스 입력 처리
                yaw += Input.GetAxis("Mouse X") * currentSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * currentSensitivity;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

                // 입력 받은 값 토대로 회전 처리
                Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
                transform.rotation = rot;

                // 만약 자동가속이면 스킵
                if (autoAcc)
                {
                    moveSpeed = speed;
                    break;
                }
                // 자동 가속 아니면 스무스하게 정지
                moveSpeed = Mathf.Lerp(minimumSpeed, speed, curAcceleration);
                break;
            default:
                UnityEngine.Debug.Log("이거 이상한데 여기 왜탐?");
                break;
        }

        // 이동 시작
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

        curSpeed = moveSpeed;
        //UnityEngine.Debug.Log($"Yaw: {yaw}, Pitch: {pitch}, moveSpeed : {moveSpeed}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Body") || other.CompareTag("Head"))
        {
            OnHitEnemy?.Invoke();
        }
    }

    IEnumerator ArrowJitter()
    {
        float harfJitterTime = chargeTime / 2f;
        float elapsed = 0f;
        float tick = UnityEngine.Random.Range(-10f, 10f);
        var originalPosition = arrowModel.transform.localPosition;
        while (elapsed < chargeTime)
        {
            elapsed += Time.deltaTime / harfJitterTime;
            tick += Time.deltaTime * chargeJitter;
            arrowModel.transform.localRotation = Quaternion.Euler(
               90f +  Mathf.PerlinNoise(tick, 0) - .5f,
                Mathf.PerlinNoise(0, tick) - .5f,
                0f);
            yield return null;
        }

        arrowModel.transform.localRotation = Quaternion.Euler(90, 0, 0); // Reset rotation
    }
}
