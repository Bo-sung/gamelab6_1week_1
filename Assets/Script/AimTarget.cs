using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Debug = UnityEngine.Debug;

public class AimTarget : MonoBehaviour
{
    [Header("Default")]
    [SerializeField] private Camera target;
    [SerializeField] private Player playerTarget;

    [SerializeField, Min(0f)]
    private float defaultDistance = 10f;

    [Header("Rotation")]
    [SerializeField, Min(0f)]
    private float sensitivity = 1f;

    [SerializeField, Range(-89f, 89f)]
    private float minPitch = -80f;

    [SerializeField, Range(-89f, 89f)]
    private float maxPitch = 80f;

    [Header("Collision")]
    [SerializeField]
    private LayerMask obstacleMask = Physics.DefaultRaycastLayers;

    // 충돌 지점에서 플레이어 방향으로 조금 물러나는 거리.
    // 오브젝트의 부피 전체를 보장하는 값은 아님.
    [SerializeField, Min(0f)]
    private float surfaceOffset = 0.05f;

    [Header("Monitor")]
    [SerializeField] private float currentDistance;
    [SerializeField] private bool isBlocked;

    float yaw, pitch = 28f;

    void Start()
    {
        if (playerTarget == null)
        {
            Debug.LogError(
                $"{nameof(AimTarget)}: playerTarget이 지정되지 않았습니다.",
                this);

            enabled = false;
            return;
        }

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // UI 등에서 커서 잠금을 해제한 동안에는 회전 입력을 받지 않음.
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;

        yaw = Mathf.Repeat(yaw, 360f);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void LateUpdate()
    {
        if (playerTarget == null)
            return;

        // 거리 계산과 배치의 기준점을 플레이어로 통일.
        Vector3 origin = playerTarget.transform.position;

        // 입력으로 계산한 회전을 배치 방향에도 사용.
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 direction = rotation * Vector3.forward;

        float maxDistance = Mathf.Max(0f, defaultDistance);
        float nearestDistance = maxDistance;

        isBlocked = false;


        // 플레이어에서 부터. 바라보는 방향으로 거리만큼 레이 발사
        var rayResult = Physics.RaycastAll(origin, direction, maxDistance, obstacleMask, QueryTriggerInteraction.Ignore);

        foreach (RaycastHit hit in rayResult)
        {
            if (ShouldIgnore(hit.collider))
                continue;

            // RaycastAll 결과 순서는 보장되지 않으므로
            // 유효한 충돌 중 가장 가까운 거리를 직접 선택.
            if (hit.distance > nearestDistance)
                continue;

            nearestDistance = hit.distance;
            isBlocked = true;
        }


        // 장애물이 있을 때만 표면에서 조금 물러남.
        // 장애물이 없다면 defaultDistance를 그대로 유지.
        currentDistance = isBlocked ? Mathf.Max(0f, nearestDistance - surfaceOffset) : maxDistance;

        // 최종 위치와 회전은 한 번만 적용.
        transform.SetPositionAndRotation(origin + direction * currentDistance, rotation);
    }


    private bool ShouldIgnore(Collider hitCollider)
    {
        if (hitCollider == null)
            return true;

        Transform hitTransform = hitCollider.transform;

        // 자신 및 자신의 자식 콜라이더 제외.
        if (hitTransform.IsChildOf(transform))
            return true;

        // 플레이어 및 플레이어의 자식 콜라이더 제외.
        if (hitTransform.IsChildOf(playerTarget.transform))
            return true;

        // 카메라 및 카메라의 자식 콜라이더 제외.
        if (target != null &&
            hitTransform.IsChildOf(target.transform))
        {
            return true;
        }

        // 기존 Arrow 태그 제외 조건 유지.
        if (hitCollider.CompareTag("Arrow"))
            return true;

        // 자식 콜라이더 + 부모 Rigidbody 구조도 고려.
        Rigidbody hitBody = hitCollider.attachedRigidbody;

        if (hitBody != null && hitBody.CompareTag("Arrow"))
            return true;

        return false;
    }

    private void OnValidate()
    {
        defaultDistance = Mathf.Max(0f, defaultDistance);
        surfaceOffset = Mathf.Max(0f, surfaceOffset);
        sensitivity = Mathf.Max(0f, sensitivity);

        if (maxPitch < minPitch)
            maxPitch = minPitch;
    }
}
