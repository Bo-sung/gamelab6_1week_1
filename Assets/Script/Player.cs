using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public interface IStatManager
{
    int GetHP();
    void SetHP(int _hp);
    void ApplyHP(int _hp);
    int GetMaxHP();
}

public class Player : MonoBehaviour
{
    public enum State
    {
        None,   // 빈손
        Holding, // 들고있음
        Aiming, // 조준중
        Flying, // 던졌음
        Returning // 돌아오는중
    }
    [SerializeField]
    IStatManager statManager;

    [SerializeField]
    PlayerController playerController;
    [SerializeField]
    ArrowController arrowController;

    [SerializeField]
    ArrowFolowCam arrowCam;
    [SerializeField]
    OrbitCamera playerCam;

    [SerializeField]
    Transform arrowHoldPosition;

    [SerializeField]
    AimTarget aimTarget;

    [SerializeField]
    State state = State.Holding;

    public int hitEffectDuration = 1;

    private float reGrabTime = 2.5f;
    private float reGrabTimer;

    public System.Action<EnemyBase> OnPlayerHit;

    [SerializeField]
    private Volume hitVolume;

    private Coroutine hitEffectCoroutine;
    private bool gameplayStopped;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();

        OnPlayerHit += OnHit;

        state = State.Holding;

        arrowController.OnHitGround += HandleArrowOnGround;

        arrowController.OnReturnComplete += HandleArrowReturned;
    }
    private void HandleArrowReturned()
    {
        if (state != State.Returning)
            return;

        state = State.Holding;

        SwitchState();
    }

    public void Initialize(IStatManager manager)
    {
        statManager = manager;
        SwitchState();
    }

    void Start()
    {
    }

    private void HandleArrowOnGround()
    {
        if (state == State.Flying)
        {
            state = State.None;
            SwitchState();
        }
    }

    void Update()
    {
        HandleInput();
    }

    void OnTriggerEnter(Collider other)
    {
        if (gameplayStopped)
            return;
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<EnemyBase>(out var enemy))
                OnPlayerHit?.Invoke(enemy);
        }
        if (other.CompareTag("Arrow"))
        {
            state = State.Holding;
            HoldingHand();
        }
    }

    private void OnHit(EnemyBase enemy)
    {
        if (gameplayStopped)
            return;
        statManager.ApplyHP(-1);

        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
        }

        if (hitVolume != null)
            hitEffectCoroutine = StartCoroutine(HitEffectSmooth());
    }

    IEnumerator HitEffectSmooth()
    {
        var time = 0f;
        while (time < hitEffectDuration)
        {
            time += Time.deltaTime;
            hitVolume.weight = Mathf.Lerp(1, 0, time / hitEffectDuration);
            yield return null;
        }
        hitVolume.weight = 0f;
        hitEffectCoroutine = null;
    }

    public void StopGameplay()
    {
        gameplayStopped = true;
        enabled = false;
        playerController.enabled = false;
        arrowController.enabled = false;
        arrowController.StopAllCoroutines();
        arrowCam.enabled = false;
        playerCam.enabled = false;
    }

    private void HandleInput()
    {
        var tempstate = state;

        // 발사 최우선
        // 그다움 조준
        // 클릭
        if (Input.GetKey(KeyCode.Mouse0))
        {
            switch (state)
            {
                case State.Aiming:
                case State.Holding:
                    tempstate = State.Flying;
                    break;
            }
        }
        // 우클릭 놓기
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            switch (state)
            {
                case State.Aiming:
                    tempstate = State.Holding;
                    break;
            }
        }
        // 우클릭
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            switch (state)
            {
                case State.Holding:
                    tempstate = State.Aiming;
                    break;
                case State.None:
                    tempstate = State.Returning;
                    break;
            }
        }
        reGrabTimer -= Time.deltaTime;
        reGrabTimer = Mathf.Clamp(reGrabTimer, 0, reGrabTime);

        if (tempstate != state)
        {
            state = tempstate;
            SwitchState();
        }
        switch (state)
        {
            case State.None: NoneTick(); break;
            case State.Holding: HoldingTick(); break;
            case State.Aiming: AimingTick(); break;
            case State.Returning: ReturningTick(); break;
            case State.Flying: FlyTick(); break;
        }

    }

    private void NoneTick()
    {

    }

    private void HoldingTick()
    {

    }

    private void AimingTick()
    {
        AimingArrow();
    }

    private void ReturningTick()
    {

    }

    private void FlyTick()
    {

    }

    private void NoneHand()
    {
        DisableArrowCon();
        DisableArrowCam();

        EnablePlayerCon();
        EnablePlayerCam();
    }
    private void HoldingHand()
    {
        arrowController.CancelReturn();

        DisableArrowCon();

        arrowController.transform.SetParent(arrowHoldPosition);

        arrowController.transform.localPosition = Vector3.zero;

        arrowController.transform.localRotation = Quaternion.identity;

        DisableArrowCam();

        EnablePlayerCon();
        EnablePlayerCam();
    }
    private void Returning()
    {
        DisableArrowCam();
        EnablePlayerCon();
        EnablePlayerCam();
        EnableArrowCon();

        if (!arrowController.StartReturn(this.transform))
        {
            Debug.LogWarning("화살 귀환 실패");

            state = State.None;
            SwitchState();
        }
    }
    private void Aim()
    {
        arrowController.CancelReturn();

        DisableArrowCon();
        EnableArrowCam();

        EnablePlayerCon();
        DisablePlayerCam();

        playerController.isAiming = true;
    }

    private void Fire()
    {
        reGrabTimer = reGrabTime;

        arrowController.CancelReturn();

        EnableArrowCon();
        EnableArrowCam();

        DisablePlayerCon();
        DisablePlayerCam();
    }

    private void DisableArrowCon()
    {
        arrowController.enabled = false;
    }

    private void EnableArrowCon()
    {
        arrowController.enabled = true;
        arrowController.transform.SetParent(null);
    }

    private void DisableArrowCam()
    {
        arrowCam.enabled = false;
    }
    private void EnableArrowCam()
    {
        arrowCam.enabled = true;
    }

    private void DisablePlayerCon()
    {
        playerController.enabled = false;
    }

    private void EnablePlayerCon()
    {
        playerController.enabled = true;
    }
    private void DisablePlayerCam()
    {
        playerCam.enabled = false;
    }

    private void EnablePlayerCam()
    {
        playerCam.enabled = true;
    }

    private void AimingArrow()
    {
        transform.LookAt(new Vector3(aimTarget.transform.position.x - 2, transform.position.y, aimTarget.transform.position.z - 2));
        arrowHoldPosition.LookAt(aimTarget.transform);
    }

    private void SwitchState()
    {
        playerController.isAiming = false;
        switch (state)
        {
            case State.Aiming:
                Aim();
                break;
            case State.Holding:
                HoldingHand();
                break;
            case State.Flying:
                Fire();
                break;
            case State.None:
                NoneHand();
                break;
            case State.Returning:
                Returning();
                break;
        }

    }
}
