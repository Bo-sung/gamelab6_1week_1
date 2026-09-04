using Unity.VisualScripting;
using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    // 플레이어는 아무것도 안함 / 움직임 / 던짐 상태. 
//    public enum PlayerState
//    {
//        None,
//        Moving,
//        Throwing,
//    }

//    [SerializeField] PlayerState state;

//    [SerializeField]
//    private float moveSpeed = 5f;
//    [SerializeField]
//    private float sensivity = 1f;
//    [SerializeField] float minPitch = -20f, maxPitch = 70f;

//    // 입력값
//    float vinput, hinput, mouseInputX, mouseInputY;

//    float yaw, pitch = 28f;

//    Vector3 moveDirection = Vector3.zero;

//    float curMoveSpeed = 0;


//    public void Initialize()
//    {

//    }

//    void Awake()
//    {
//    }

//    // 입력 처리와 이동 처리 및 시선 처리는 항상 여기서 같이. 만약 이동 못하게 할거면 이동 관련 변수 0으로 세팅
//    void Update()
//    {
//        HandleInput();
//        HandleViewport();
//        switch (state)
//        {
//            case PlayerState.None: HandleNoneUpdate(); break;
//            case PlayerState.Moving: HandleMovingUpdate(); break;
//            case PlayerState.Throwing: HandleThrowingUpdate(); break;
//        }
//        HandleMovement();
//    }

//    private void HandleInput()
//    {
//        vinput = Input.GetAxis("Vertical");
//        hinput = Input.GetAxis("Horizontal");
//        mouseInputX = Input.GetAxis("Mouse X");
//        mouseInputY = Input.GetAxis("Mouse Y");
//    }

//    private float verticalRotation = 0f;

//    private void HandleViewport()
//    {
//        // 2. 시선 전환 분배 처리

//        float mX = mouseInputX * sensivity;
//        float mY = mouseInputY * sensivity;

//        // 좌우 회전: 캐릭터(부모) 몸통을 회전
//        transform.Rotate(Vector3.up * mX);
//    }

//    private void HandleMovement()
//    {
//        // 입력 받은 값 토대로 회전 처리
//        //transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

//        // 이동 시작
//        transform.Translate(moveDirection * Time.deltaTime * curMoveSpeed);
//    }

//    private void HandleNoneUpdate()
//    {
//        moveDirection = new Vector3(hinput, 0, vinput).normalized;
//        curMoveSpeed = moveSpeed;
//    }

//    private void HandleMovingUpdate()
//    {
//        moveDirection = new Vector3(hinput, 0, vinput).normalized;
//        curMoveSpeed = moveSpeed;
//    }

//    private void HandleThrowingUpdate()
//    {

//    }
//}

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Grounded,
        Airborne,
        Dashing,
        Slamming,
        stunned,
    }

    [Header("Camera")]
    [SerializeField] Transform cameraTransform;

    [Header("Move")]
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float rotationSpeed = 12f;

    [Header("Jump / Gravity")]
    [SerializeField] float jumpHeight = 2.2f;
    [SerializeField] float gravity = -30f;
    [SerializeField] float fallGravityMultiplier = 1.5f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float jumpBufferTime = 0.12f;

    [Header("Dash")]
    [SerializeField] float dashSpeed = 15f;
    [SerializeField] float dashMaxDuration = 0.5f;   // 유지 가능한 최대 시간(실시간)
    [SerializeField] float dashCooldown = 0.6f;
    [SerializeField] float dashTimeScale = 0.25f;    // 대시 중 세계 속도

    [Header("Slam (내리찍기)")]
    [SerializeField] float slamHangTime = 0.1f;   // 급강하 전 멈칫
    [SerializeField] float slamFallSpeed = -40f;
    [SerializeField] float slamStunTime = 0.35f;  // 헛찍기 경직
    public PlayerState CurrentState { get; private set; } = PlayerState.Grounded;
    public bool IsSlamming => CurrentState == PlayerState.Slamming;
    public bool IsFallingOnEnemy => velocity.y < -0.1f || IsSlamming; // 밟기 판정 조건
    public System.Action OnSlamImpact;   // 내리찍기로 바닥에 닿은 순간 (충격파용 훅)
    public static float BaseTimeScale { get; private set; } = 1f;

    CharacterController cc;
    Vector3 velocity;                 // y만 중력 누적, xz는 매 프레임 계산
    Vector3 dashDirection;
    Vector3 lastMoveDirection = Vector3.forward;

    float coyoteCounter;
    float jumpBufferCounter;
    float dashTimer;
    float dashCooldownTimer;
    float slamHangTimer;
    float stunTimer;
    bool hasAirDash;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        ReadTimers();

        switch (CurrentState)
        {
            case PlayerState.Grounded: TickGrounded(); break;
            case PlayerState.Airborne: TickAirborne(); break;
            case PlayerState.Dashing: TickDashing(); break;
            case PlayerState.Slamming: TickSlamming(); break;
            case PlayerState.stunned: TickStunned(); break;
        }
    }


    void ReadTimers()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        dashCooldownTimer -= Time.unscaledDeltaTime;
    }

    Vector3 GetCameraRelativeInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 f = cameraTransform.forward; f.y = 0f; f.Normalize();
        Vector3 r = cameraTransform.right; r.y = 0f; r.Normalize();
        Vector3 dir = (f * v + r * h);
        return dir.sqrMagnitude > 1f ? dir.normalized : dir;
    }
    bool DashPressed() => Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f;


    private void TickStunned()
    {
        stunTimer -= Time.deltaTime;
        velocity.y = -2f;

        if (stunTimer <= 0f)
        {
            CurrentState = cc.isGrounded ? PlayerState.Grounded : PlayerState.Airborne;
        }
    }

    private void TickSlamming()
    {
        if (slamHangTimer > 0f)
        {
            slamHangTimer -= Time.deltaTime;
            return;
        }

        velocity.y = slamFallSpeed;
        cc.Move(velocity * Time.deltaTime);

        if (cc.isGrounded)
        {
            OnSlamImpact?.Invoke();
            EnterStun();
        }
    }

    void TickDashing()
    {
        float dt = Time.unscaledDeltaTime;

        dashTimer -= dt;
        cc.Move(dashDirection * dashSpeed * dt);

        if (dashTimer <= 0f)
        {
            ExitDash();
            CurrentState = cc.isGrounded ? PlayerState.Grounded : PlayerState.Airborne;
        }
    }

    private void TickAirborne()
    {
        coyoteCounter -= Time.deltaTime;

        // 코요테 타임 내 점프 허용
        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            DoJump(); return;
        }

        // 공중에서 Space 재입력 = 내리찍기 (상승 중이든 하강 중이든 즉시)
        if (jumpBufferCounter > 0f && coyoteCounter <= 0f)
        {
            EnterSlam(); return;
        }

        if (DashPressed() && hasAirDash)
        {
            hasAirDash = false;
            EnterDash(GetCameraRelativeInput());
            return;
        }

        ApplyGravity();
        Vector3 input = GetCameraRelativeInput();
        MoveAndRotate(input * moveSpeed);

        if (cc.isGrounded)
            CurrentState = PlayerState.Grounded;
    }

    private void TickGrounded()
    {
        coyoteCounter = coyoteTime;
        hasAirDash = true;
        velocity.y = -2f;                       // 바닥에 붙이는 힘

        Vector3 input = GetCameraRelativeInput();
        MoveAndRotate(input * moveSpeed);

        if (jumpBufferCounter > 0f)
        {
            DoJump(); return;
        }
        if (DashPressed())
        {
            EnterDash(input); return;
        }
        if (!cc.isGrounded)
        {
            CurrentState = PlayerState.Airborne;
        }
    }

    void DoJump()
    {
        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
        velocity.y = Mathf.Sqrt(2f * -gravity * jumpHeight);
        CurrentState = PlayerState.Airborne;
    }
    void EnterDash(Vector3 inputDir)
    {
        dashDirection = inputDir.sqrMagnitude > 0.01f ? inputDir : lastMoveDirection;
        dashDirection.y = 0f;
        dashDirection.Normalize();
        transform.rotation = Quaternion.LookRotation(dashDirection);

        dashTimer = dashMaxDuration;
        dashCooldownTimer = dashCooldown;
        velocity.y = 0f;
        CurrentState = PlayerState.Dashing;

        BaseTimeScale = dashTimeScale;
        Time.timeScale = dashTimeScale;
        Time.fixedDeltaTime = 0.02f * dashTimeScale;
    }

    void EnterSlam()
    {
        jumpBufferCounter = 0f;
        slamHangTimer = slamHangTime;
        velocity = Vector3.zero;
        CurrentState = PlayerState.Slamming;
    }

    void EnterStun()
    {
        stunTimer = slamStunTime;
        velocity.y = -2f;
        CurrentState = PlayerState.stunned;
    }
    public void Bounce(float height)
    {
        velocity.y = Mathf.Sqrt(2f * -gravity * height);
        slamHangTimer = 0f;
        hasAirDash = true;
        CurrentState = PlayerState.Airborne;
    }
    void MoveAndRotate(Vector3 horizontalVelocity)
    {
        if (horizontalVelocity.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = horizontalVelocity.normalized;
            Quaternion targetRot = Quaternion.LookRotation(lastMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        Vector3 motion = horizontalVelocity;
        motion.y = velocity.y;
        cc.Move(motion * Time.deltaTime);
    }

    void ApplyGravity()
    {
        float g = velocity.y < 0f ? gravity * fallGravityMultiplier : gravity;
        velocity.y += g * Time.deltaTime;
    }
    void ExitDash()
    {
        BaseTimeScale = 1f;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void OnDisable() => ExitDash();
}
