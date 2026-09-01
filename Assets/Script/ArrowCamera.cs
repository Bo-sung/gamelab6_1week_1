using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Rigidbody _targetRb;

    [SerializeField]
    private Vector3 Offset = new Vector3(0, 1, -3);
    [SerializeField]
    private float SmoothTime = 0.1f;

    private Camera _cam;
    private Vector3 _currentVelocity;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        ApplyViewImmediate();
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        
        Vector3 targetWorldPos = _target.TransformPoint(Offset);

 
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetWorldPos,
            ref _currentVelocity,
            SmoothTime
        );

        Vector3 lookTarget = _target.transform.position;
        transform.LookAt(lookTarget);

    }

    private void ApplyViewImmediate()
    {
        transform.position = _target.TransformPoint(Offset);
        transform.LookAt(_target.transform.position);
        _currentVelocity = Vector3.zero;
    }
}