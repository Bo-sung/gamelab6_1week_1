using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;


public class Player : MonoBehaviour
{
    public enum State
    {
        Holding, // 들고있음
        Flying, // 던졌음
        None,   // 빈손인데 주우러 가야함
    }

    [SerializeField]
    PlayerController controller;
    [SerializeField]
    ArrowController arrow;

    [SerializeField]
    ArrowFolowCam arrowFollowCam;
    [SerializeField]
    OrbitCamera playerCam;


    [SerializeField]
    Transform arrowHoldPosition;

    [SerializeField]
    State state = State.Holding;

    public int hitEffectDuration = 1;

    public System.Action<EnemyBase> OnPlayerHit;

    [SerializeField]
    private Volume hitVolume;

    private Coroutine hitEffectCoroutine;

    private int curHealth = 10;
    private int maxHealth = 10;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        OnPlayerHit += OnHit;

        curHealth = maxHealth;
        state = State.Holding;
        arrow.OnHitGround += HandleArrowOnGround;
    }

    void Start()
    {
        SwitchState();
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
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<EnemyBase>(out var enemy))
                OnPlayerHit?.Invoke(enemy);
        }
    }

    private void OnHit(EnemyBase enemy)
    {
        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
        }

        StartCoroutine(HitEffectSmooth());
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
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var tempstate = state;
            switch (state)
            {
                case State.Holding:
                    tempstate = State.Flying;
                    break;
                case State.None:
                    tempstate = State.Holding;
                    break;
            }

            if (tempstate != state)
            {
                state = tempstate;
                SwitchState();
            }
        }
    }

    public void Heal()
    {
        curHealth += 1;
    }

    private void HoldArrow()
    {
        arrow.transform.SetParent(arrowHoldPosition);
        arrow.transform.localPosition = arrowHoldPosition.position;
        arrow.transform.rotation = arrowHoldPosition.rotation;
        arrow.enabled = false;
        arrow.MakeStatic();


        arrowFollowCam.enabled = false;

        playerCam.enabled = true;
        controller.enabled = true;
    }

    private void ReleaseArrow()
    {
        arrow.transform.SetParent(null);
        arrow.enabled = true;
        arrow.MakeDynamic();

        arrowFollowCam.enabled = true;

        playerCam.enabled = false;
        controller.enabled = false;

    }
    private void NoneState()
    {
        arrow.transform.SetParent(null);
        arrow.enabled = false;

        arrowFollowCam.enabled = false;

        playerCam.enabled = true;
        controller.enabled = true;
    }

    private void SwitchState()
    {
        switch (state)
        {
            case State.Holding:
                HoldArrow();
                break;
            case State.Flying:
                ReleaseArrow();
                break;
            case State.None:
                NoneState();
                break;
        }

    }
}
