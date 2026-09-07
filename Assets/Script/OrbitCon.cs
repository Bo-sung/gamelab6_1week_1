using UnityEngine;

public class OrbitCon : MonoBehaviour
{
    [Header("Orbit")]
    [SerializeField] float sensitivity = 3f;
    [SerializeField] float minPitch = -20f, maxPitch = 70f;

    [Header("Collision")]
    [SerializeField] LayerMask collisionMask;
    [SerializeField] float probeRadius = 0.35f;
    [SerializeField] float recoverSpeed = 6f;

    float yaw, pitch = 28f;
    float currentDistance;
    Vector3 followPoint;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        followPoint.x = transform.position.x;
        followPoint.z = transform.position.z;
        followPoint.y = transform.position.y;

        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 dir = rot * Vector3.back;

        // 벽관통 방지
        float targetDistance = 5f;
        if (Physics.SphereCast(followPoint, probeRadius, dir, out RaycastHit hit, 5f, collisionMask, QueryTriggerInteraction.Ignore))
            targetDistance = hit.distance;

        targetDistance = Mathf.Clamp(targetDistance, 0, 5f);

        if (targetDistance < currentDistance)
            currentDistance = targetDistance;
        else
            currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, recoverSpeed * Time.deltaTime);

        transform.SetPositionAndRotation(followPoint + dir * currentDistance, rot);
    }
}
