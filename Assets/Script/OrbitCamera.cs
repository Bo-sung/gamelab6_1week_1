using UnityEngine;
/// <summary>
/// 마우스 궤도 3인칭 카메라.
/// </summary>
public class OrbitCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;
    [SerializeField] Transform target_aimMode;
    [SerializeField] float verticalSmoothTime = 0.12f;
    [SerializeField] PlayerController playerController;

    [Header("Orbit")]
    [SerializeField] float distance = 7f;
    [SerializeField] float minDistance = 1.2f;
    [SerializeField] float sensitivity = 3f;
    [SerializeField] float minPitch = -20f, maxPitch = 70f;
    [SerializeField] float Aimdistance = 4f;

    [Header("Collision")]
    [SerializeField] LayerMask collisionMask;
    [SerializeField] float probeRadius = 0.35f;
    [SerializeField] float recoverSpeed = 6f;

    [Header("Dash FOV")]
    [SerializeField] float normalFov = 60f;
    [SerializeField] float dashFov = 78f;
    [SerializeField] float fovLerpSpeed = 12f;
    [SerializeField] float AimFov = 40f;

    public bool IsAim;

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
        if (!IsAim)
            followPoint = target.position;
        else
            followPoint = target_aimMode.position;

        var result = CommonUtils.ReCalcInput(transform.position);
        pitch = result.pitch;
        yaw = result.yaw;
    }

    void LateUpdate()
    {
        if (!IsAim)
        {
            followPoint.x = target.position.x;
            followPoint.z = target.position.z;
            followPoint.y = Mathf.SmoothDamp(followPoint.y, target.position.y, ref yVelocity, verticalSmoothTime);
        }
        else
        {
            followPoint.x = target_aimMode.position.x;
            followPoint.z = target_aimMode.position.z;
            followPoint.y = target_aimMode.position.y;
        }

        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 dir = rot * Vector3.back;

        // 벽관통 방지
        float targetDistance = distance;
        if (Physics.SphereCast(followPoint, probeRadius, dir, out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
            targetDistance = hit.distance;

        if (!IsAim)
            targetDistance = Mathf.Clamp(targetDistance, minDistance, distance);
        else
            targetDistance = Mathf.Clamp(targetDistance, minDistance, Aimdistance);

        if (targetDistance < currentDistance)
            currentDistance = targetDistance;
        else
            currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, recoverSpeed * Time.deltaTime);

        transform.SetPositionAndRotation(followPoint + dir * currentDistance, rot);
        bool dashing = playerController != null && playerController.CurrentState == PlayerController.PlayerState.Dashing;

        if (dashing)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, dashFov, fovLerpSpeed * Time.unscaledDeltaTime);
        else if (IsAim)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, AimFov, fovLerpSpeed * Time.unscaledDeltaTime);
        else
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normalFov, fovLerpSpeed * Time.unscaledDeltaTime);
    }
}
