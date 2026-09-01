using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] float sensitivity = 3f;
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
    // 대시 게이지 회복 시간
    [SerializeField] float dashGaugeRegenTime = 5f;
    float remainDashGaugeRegenTime = 0f;

    public System.Action OnDashStart;

    float yaw, pitch = 28f;
    
    public bool IsDashing = false;
    public bool IsDashAvailable => curDashGauge > 0 && remainDashTime <= 0;

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
        // 대시 지속시간 체크
        if (remainDashTime <= 0)
            IsDashing = false;

        // 대시 키 체크
        // AND 대시 게이지 체크
        // AND 대시 지속시간 체크
        if (Input.GetKeyDown(KeyCode.LeftShift) && curDashGauge > 0 && remainDashTime <= 0)
        {
            DoDash();
        }

        // 대시 게이지 회복 체크
        // 대시 잔여량 없는데 게이지 리젠 타임이 0 이하이면 게이지 회복
        if (curDashGauge <= 0 && remainDashGaugeRegenTime <= 0)
        {
            curDashGauge = dashGaugeMax;
        }

        // 대시 지속시간, 쿨타임, 게이지 회복 타이머 감소
        remainDashTime -= Time.deltaTime;
        remainDashCooldown -= Time.deltaTime;
        remainDashGaugeRegenTime -= Time.deltaTime;

    }

    private void DoDash()
    {
        IsDashing = true;
        remainDashTime = dashTime;
        remainDashCooldown = dashCooldown;
        curDashGauge--;
        OnDashStart?.Invoke();
        // 대시 게이지가 0이 되면 게이지 회복 타이머를 시작
        if (curDashGauge <= 0)
        {
            remainDashGaugeRegenTime = dashGaugeRegenTime;
        }
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        transform.Translate(Vector3.forward * Time.deltaTime * (IsDashing ? dashSpeed : speed));
        transform.rotation = rot;
        Debug.Log($"Yaw: {yaw}, Pitch: {pitch}");
    }
}
