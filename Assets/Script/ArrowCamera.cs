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
    private Vector3 DashOffset = new Vector3(0, 1, -5);
    [SerializeField]
    private float SmoothTime = 0.1f;

    
    private Camera _cam;
    private Vector3 _currentVelocity;
    private Vector3 currentOffset;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        ApplyViewImmediate();
        currentOffset = Offset;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        
        Vector3 targetWorldPos = target.TransformPoint(controller.IsDashing? DashOffset:Offset);

 
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


}