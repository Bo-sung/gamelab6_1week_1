using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Animations;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody targetRb;
    [SerializeField] private ArrowController controller;

    [Header("Value Setting")]
    [SerializeField]
    private Vector3 Offset = new Vector3(0, 1, -3);
    [SerializeField]
    private float SmoothTime = 0.1f;
    [SerializeField]
    private float ChargingFov = 40f;
    [SerializeField]
    private float DashFov = 70f;
    [SerializeField]
    private float NormalToChargeTime = 0.1f;
    [SerializeField]
    private float ChargeToDashTime = 0.1f;
    [SerializeField]
    private float DashToNormalTime = 0.4f;
   
    
    [SerializeField]
    private float RemainDashTime = 0.2f;
    [SerializeField]


    private Camera cam;
    private Vector3 _currentVelocity;

    private float defaultFov;

    private float currentFov { get { return cam.fieldOfView; }
        set { cam.fieldOfView = value; }}

    private ArrowController.DashState dashState = ArrowController.DashState.None;
    private float remainDashCount;

    private Coroutine fovChangeCoroutine;

    private void Start()
    {
        cam = GetComponent<Camera>();
        ApplyViewImmediate();

        defaultFov = cam.fieldOfView;

        controller.OnDashStart += OnDash;
        controller.OnHitEnemy += OnHitEnemy;
        controller.OnDashStateChanged += OnArrowDashStateChanged;

    }

    private void LateUpdate()
    {
        if (target == null) return;

    
        //cam.fieldOfView = Mathf.Lerp(DashFov, DefaultFov, remainDashCount / RemainDashTime);
        //remainDashCount += Time.deltaTime;
    
        

        Vector3 targetWorldPos = target.TransformPoint(Offset);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetWorldPos,
            ref _currentVelocity,
            SmoothTime
        );

        Vector3 lookTarget = target.transform.position;
        transform.LookAt(lookTarget);

    }

    private void ApplyViewImmediate()
    {
        transform.position = target.TransformPoint(Offset);
        transform.LookAt(target.transform.position);
        _currentVelocity = Vector3.zero;
    }

    private void OnDash()
    {
        cam.fieldOfView = DashFov;
        remainDashCount = 0;

    }

    private void OnHitEnemy()
    {
 


    }

    private void OnArrowDashStateChanged(ArrowController.DashState state)
    {
        if (fovChangeCoroutine != null) StopCoroutine(fovChangeCoroutine);

        switch (state)
        {
            //차징상태 진입
            case ArrowController.DashState.Charging:
                StartCoroutine(FovChangeSmooth(defaultFov, ChargingFov, NormalToChargeTime));
                break;

            case ArrowController.DashState.Dash:
                StartCoroutine(FovChangeSmooth(ChargingFov, DashFov, ChargeToDashTime));
                break;
            case ArrowController.DashState.Cooldown:
                StartCoroutine(FovChangeSmooth(DashFov, defaultFov, DashToNormalTime));
                break;


        }
    }

    IEnumerator FovChangeSmooth(float originFov, float targetFov, float time)
    {
        var count = 0f;
        while(count < time)
        {
            count += Time.deltaTime;
            currentFov = Mathf.Lerp(originFov, targetFov, count / time);
            yield return null;
        }
        
    }


    


}