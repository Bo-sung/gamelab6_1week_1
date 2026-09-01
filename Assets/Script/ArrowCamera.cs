using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody targetRb;
    [SerializeField] private ArrowController controller;

    [SerializeField]
    private Vector3 Offset = new Vector3(0, 1, -3);
    [SerializeField]
    private float DashFov = 70f;


    [SerializeField]
    private float SmoothTime = 0.1f;
    [SerializeField]
    private float RemainDashTime = 0.2f;


    private Camera cam;
    private Vector3 _currentVelocity;

    private float DefaultFov;

    private bool isDashing;
    private float remainDashCount;
    private Coroutine DashBackCo;

    private void Start()
    {
        cam = GetComponent<Camera>();
        ApplyViewImmediate();

        DefaultFov = cam.fieldOfView;

        controller.OnDashStart += OnDash;

    }

    private void LateUpdate()
    {
        if (target == null) return;

    
        cam.fieldOfView = Mathf.Lerp(DashFov, DefaultFov, remainDashCount / RemainDashTime);
        remainDashCount += Time.deltaTime;
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

    public void OnDash()
    {

        cam.fieldOfView = DashFov;
        remainDashCount = 0;


    }




}