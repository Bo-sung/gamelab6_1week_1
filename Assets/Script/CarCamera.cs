using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CarCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject Target;

    [SerializeField] private Transform _target;
    [SerializeField] private Rigidbody _targetRb;

    [SerializeField]
    private Material WallRed;
    [SerializeField] 
    private Material WallTrans;


    [SerializeField]
    private Vector3 Offset;
    [SerializeField]
    private float SmoothTime = 0.1f;

    private Camera _cam;
    private Vector3 _currentVelocity;

    private void Start()
    {
        _cam = GetComponent<Camera>();
        //임시 함수
        _target = Target.transform;
        _targetRb = Target.GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        // --- 위치 이동 (Follow) ---
        // 타겟의 회전을 반영한 월드 좌표 계산
        // TransformPoint: 로컬 좌표(Offset)를 월드 좌표로 변환
        Vector3 targetWorldPos = _target.TransformPoint(Offset);

        // SmoothDamp로 부드럽게 이동
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetWorldPos,
            ref _currentVelocity,
            SmoothTime
        );

        // --- 회전 (Look At) ---
        // 차량의 특정 지점을 바라봄
        Vector3 lookTarget = _target.transform.position;
        transform.LookAt(lookTarget);

       
    }


    public void SetTarget(Transform targetTransform, Rigidbody targetRigidbody)
    {
        _target = targetTransform;
        _targetRb = targetRigidbody;

        // 타겟이 바뀌면 카메라를 즉시 해당 위치로 이동 (텔레포트)하여 튀는 현상 방지
        if (_target != null)
        {
            ApplyViewImmediate();
        }
    }


    private void ApplyViewImmediate()
    { 
        transform.position = _target.TransformPoint(Offset);
        transform.LookAt(_target.transform.position);
        _currentVelocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Wall"))
        {
            var renderer = other.GetComponent<MeshRenderer>();
            renderer.material = WallTrans;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Wall"))
        {
            var renderer = other.GetComponent<MeshRenderer>();
            renderer.material = WallRed;
        }

    }
}