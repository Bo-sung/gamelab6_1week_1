using UnityEngine;
/// <summary>
/// 마우스 궤도 3인칭 카메라.
/// </summary>
public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;
    [SerializeField] float verticalSmoothTime = 0.12f;
    [SerializeField] PlayerController playerController;

    [Header("Orbit")]
    [SerializeField] float distance = 7f;
    [SerializeField] float minDistance = 1.2f;
    [SerializeField] float sensitivity = 3f;
    [SerializeField] float minPitch = -20f, maxPitch = 70f;

    [Header("Collision")]
    [SerializeField] LayerMask collisionMask;
    [SerializeField] float probeRadius = 0.35f;
    [SerializeField] float recoverSpeed = 6f;

    [Header("Dash FOV")]
    [SerializeField] float normalFov = 60f;
    [SerializeField] float dashFov = 78f;
    [SerializeField] float fovLerpSpeed = 12f;


    Camera cam;
    void Awake() => cam = GetComponent<Camera>();

    float yaw, pitch = 28f;
    float currentDistance;
    Vector3 followPoint;
    float yVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentDistance = distance;
        followPoint = target.position;
    }

    void LateUpdate()
    {
        followPoint.x = target.position.x;
        followPoint.z = target.position.z;
        followPoint.y = Mathf.SmoothDamp(followPoint.y, target.position.y, ref yVelocity, verticalSmoothTime);

        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 dir = rot * Vector3.back;

        // 벽관통 방지
        float targetDistance = distance;
        if (Physics.SphereCast(followPoint, probeRadius, dir, out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
            targetDistance = hit.distance;

        targetDistance = Mathf.Clamp(targetDistance, minDistance, distance);

        if (targetDistance < currentDistance)
            currentDistance = targetDistance;
        else
            currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, recoverSpeed * Time.deltaTime);

        transform.SetPositionAndRotation(followPoint + dir * currentDistance, rot);
        bool dashing = playerController != null && playerController.CurrentState == PlayerController.PlayerState.Dashing;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, dashing ? dashFov : normalFov, fovLerpSpeed * Time.unscaledDeltaTime);
    }
}