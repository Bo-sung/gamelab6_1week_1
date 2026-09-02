using System.Diagnostics;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] float sensitivity = 3f;
    float currentSensitivity = 3f;
    [SerializeField] float minPitch = -20f, maxPitch = 70f;
    [SerializeField] float speed = 2f;
    [SerializeField] float dashSpeed = 4f;
    [SerializeField] int curDashGauge = 3;
    [SerializeField] int dashGaugeMax = 3;
    // 대시 지속시간
    [SerializeField] float dashTime = 1f;
    float remainDashTime = 0f;
    // 대시 발동 후 다음 쿨타임
    [SerializeField] float dashCooldown = 2f;
    float remainDashCooldown = 0f;


    [SerializeField] float dashTimeScale = 0f;
    float defaultTimeScale = 1f;
    [SerializeField] float dashTimeSensitivity = 0f;
    [SerializeField] float dashChargingTime = 0f;

    public System.Action OnDashStart;
    public System.Action OnHitEnemy;

    float yaw, pitch = 28f;

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
    public bool IsDashing = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleDashLogic();
    }

    private void HandleDashLogic()
    {
        // 대시 상태 전환
        if (dashState == DashState.Dashing && remainDashTime <= 0)
            dashState = DashState.Cooldown;
        else if (dashState == DashState.Cooldown && remainDashCooldown <= 0)
            dashState = DashState.None;
        else if (Input.GetKeyUp(KeyCode.Mouse1) && dashState == DashState.Charging)
            dashState = DashState.Dash;
        else if (Input.GetKeyDown(KeyCode.Mouse1) && dashState == DashState.None)
            dashState = DashState.Ready;

        ProcessDash();

        // 타이머 감소
        remainDashTime -= Time.deltaTime;
        remainDashCooldown -= Time.deltaTime;
        UnityEngine.Debug.Log($"DashState: {dashState}, remainDashTime: {remainDashTime}, remainDashCooldown: {remainDashCooldown}, IsDashing: {IsDashing}");
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
                dashState = DashState.Charging;
                dashChargingTime = 0;
                break;
            // 차징 중
            case DashState.Charging:
                // 차징중 시간 느려짐 효과 + 민감도 증가(시간이 늘어짐에 따라. 유저 입력에 보정을 주기 위해)
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
        dashState = DashState.Dashing;
        remainDashTime = dashTime;
        remainDashCooldown = dashCooldown;

        Time.timeScale = defaultTimeScale;
        currentSensitivity = sensitivity;
        OnDashStart?.Invoke();
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * currentSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * currentSensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        transform.Translate(Vector3.forward * Time.deltaTime * (dashState == DashState.Dashing ? dashSpeed * dashChargingTime : speed));
        if (dashState != DashState.Dashing)
            transform.rotation = rot;
        UnityEngine.Debug.Log($"Yaw: {yaw}, Pitch: {pitch}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Body")|| other.CompareTag("Head"))
        {
            OnHitEnemy?.Invoke();
        }
    }
}
