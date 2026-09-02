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
    // ��� ���ӽð�
    [SerializeField] float dashTime = 1f;
    float remainDashTime = 0f;
    // ��� �ߵ� �� ���� ��Ÿ��
    [SerializeField] float dashCooldown = 2f;
    float remainDashCooldown = 0f;
    [SerializeField] float dashTimeScale = 0f;
    float defaultTimeScale = 1f;
    [SerializeField] float dashTimeSensitivity = 0f;
    [SerializeField] float chargeTime = 2f;
    [SerializeField] float chargeJitter = 0.5f;

    [Header("Collision")]
    [SerializeField] float offControlTime = 0.5f;

    [Header("Monitor")]
    [SerializeField] float dashChargingTime = 0f;
    [SerializeField] float curSpeed = 0f;

    public System.Action OnDashStart;
    public System.Action<DashState> OnDashStateChanged;
    public System.Action OnHitEnemy;

    float yaw, pitch = 28f;

    private float chargeTimer = 0f;
    private float offControlTimer = 0f;
    private float offControlTimerF = 0f;
    private Rigidbody rb;
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
        rb = GetComponent<Rigidbody>();
        var arrowChargeLaser = GetComponent<ArrowChargeLaser>();
        arrowChargeLaser.Initialize(this);
    }

    private void Update()
    {
        HandleAcceleration();
        HandleDashLogic();
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
        // ��� ���� ��ȯ
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

        // Ÿ�̸� ����
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
            // ��� ���
            case DashState.Dash:
                // ��� ���� �� Ÿ�ӽ����ϰ� �ΰ��� ������� ����
                DoDash();
                break;
            // ��¡ ����
            case DashState.Ready:
                // ���� ��ȯ
                ChangeDashState(DashState.Charging);
                dashChargingTime = 0;
                chargeTimer = 0f;
                if (arrowJitterCoroutine != null) StopCoroutine(arrowJitterCoroutine);
                arrowJitterCoroutine = StartCoroutine(ArrowJitter());


                break;
            // ��¡ ��
            case DashState.Charging:
                // ��¡�� �ð� ������ ȿ�� + �ΰ��� ����(�ð��� �þ����� ����. ���� �Է¿� ������ �ֱ� ����)
                chargeTimer += Time.unscaledDeltaTime;
                dashChargingTime += Time.deltaTime;
                Time.timeScale = Mathf.Lerp(defaultTimeScale, dashTimeScale, dashChargingTime);
                currentSensitivity = Mathf.Lerp(sensitivity, dashTimeSensitivity, dashChargingTime);
                break;
            // ��� ��
            case DashState.Dashing:
                break;
            // ��Ÿ�� ��
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

        //��鸲 ����
        if (arrowJitterCoroutine != null)
        {
            StopCoroutine(arrowJitterCoroutine);
            arrowJitterCoroutine = null;
            arrowModel.transform.localRotation = Quaternion.Euler(90, 0, 0); // Reset rotation
        }
        //Ÿ�̸� �ʱ�ȭ
        chargeTimer = 0f;

        UnityEngine.Debug.Log("Dash executed!");
    }

    void LateUpdate()
    {
        offControlTimerF = offControlTimer;
        offControlTimer -= Time.deltaTime;

        // ��� ��, ���� ó��
        float moveSpeed = 0;
        switch (dashState)
        {
            case DashState.Dashing:
                moveSpeed = dashSpeed * dashChargingTime;
                break;
            case DashState.Cooldown:
            case DashState.None:
            case DashState.Charging:
                // ��ø� �ƴϸ� ȸ�� ����
                // ���콺 �Է� ó��

                if (offControlTimerF > 0 && offControlTimer <= 0)
                {
                    yaw = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
                    pitch = -Mathf.Asin(Mathf.Clamp(transform.forward.y, -1f, 1f)) * Mathf.Rad2Deg;
                }

                //�Է� ���� �� ���� ȸ�� ó��

                if (offControlTimer < 0)
                {
                    yaw += Input.GetAxis("Mouse X") * currentSensitivity;
                    pitch -= Input.GetAxis("Mouse Y") * currentSensitivity;

                    pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
                    transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
                }
                //Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
                //transform.rotation = rot;

                // ���� �ڵ������̸� ��ŵ
                if (autoAcc)
                {
                    moveSpeed = speed;
                    break;
                }
                // �ڵ� ���� �ƴϸ� �������ϰ� ����
                moveSpeed = Mathf.Lerp(minimumSpeed, speed, curAcceleration);
                break;
            default:
                UnityEngine.Debug.Log("�̰� �̻��ѵ� ���� ��Ž?");
                break;
        }


        // �̵� ����
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

        curSpeed = moveSpeed;
        UnityEngine.Debug.Log($"Yaw: {yaw}, Pitch: {pitch}, moveSpeed : {moveSpeed}");

        //�ٴ� �Ʒ��� �̵��ϴ� ���� ����
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
            tick += Time.unscaledDeltaTime * chargeJitter;
            arrowModel.transform.localRotation = Quaternion.Euler(
               90f + (Mathf.PerlinNoise(tick, 0) - .5f) * elapsed * 10,
                (Mathf.PerlinNoise(0, tick) - .5f) * elapsed * 10,
                0f);
            yield return new WaitForSecondsRealtime(0.01f);
        }

        arrowModel.transform.localRotation = Quaternion.Euler(90, 0, 0); // Reset rotation
    }

    private void OnCollisionEnter(Collision collision)
    {
        var reflectDir = Vector3.Reflect(transform.forward, collision.contacts[0].normal);
        //transform.rotation = Quaternion.LookRotation(reflectDir);
        //transform.forward = reflectDir;
        rb.rotation = Quaternion.Euler(reflectDir);
        //yaw = rb.rotation.eulerAngles.x;
        //pitch = rb.rotation.eulerAngles.y;


        offControlTimer = offControlTime;
        

    }
}
