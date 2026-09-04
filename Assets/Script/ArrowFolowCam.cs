using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowFolowCam : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    private ArrowController controller;
    [SerializeField]
    private Vector3 positionGap;
    [SerializeField]
    float verticalSmoothTime = 0.12f;

    [Header("Orbit")]
    [SerializeField]
    float distance = 7f;
    [SerializeField]
    float minDistance = 1.2f;

    [Header("Collision")]
    [SerializeField]
    LayerMask collisionMask;
    [SerializeField]
    float probeRadius = 0.35f;
    [SerializeField]
    float recoverSpeed = 6f;

    [Header("Dash FOV")]
    [SerializeField]
    float normalFov = 60f;
    [SerializeField]
    float dashFov = 30f;

    Camera cam;
    void Awake() => cam = GetComponent<Camera>();

    float currentDistance;
    Vector3 followPoint;
    float xVelocity;
    float yVelocity;
    float zVelocity;

    bool charging = false;
    float dashChargeTime;
    [SerializeField]
    float dashChargeTimer;

    void Start()
    {
        currentDistance = distance;
        followPoint = controller.transform.position;
        controller.OnDashStateChanged += HandleDashState;
        dashChargeTime = controller.ChargeTime;
    }

    private void HandleDashState(ArrowController.DashState dashState)
    {
        switch (dashState)
        {
            case ArrowController.DashState.Ready:
                charging = true;
                break;
            case ArrowController.DashState.Dash:
                charging = false;
                break;
        }
    }


    void LateUpdate()
    {
        // 차지시 pov 값 적용
        if (charging)
            dashChargeTimer += Time.deltaTime;
        else
            dashChargeTimer -= Time.deltaTime * 2;
        dashChargeTimer = Mathf.Clamp01(dashChargeTimer);
        cam.fieldOfView = Mathf.Lerp(normalFov, dashFov, dashChargeTimer);

        // 카메라 댐핑
        var target = controller.transform;
        followPoint.x = Mathf.SmoothDamp(followPoint.x, target.position.x + positionGap.x, ref xVelocity, verticalSmoothTime);
        followPoint.z = Mathf.SmoothDamp(followPoint.z, target.position.z + positionGap.z, ref zVelocity, verticalSmoothTime);
        followPoint.y = Mathf.SmoothDamp(followPoint.y, target.position.y + positionGap.y, ref yVelocity, verticalSmoothTime);
        var dir = controller.transform.rotation * Vector3.back;

        // 벽관통 방지
        float targetDistance = distance;
        if (Physics.SphereCast(followPoint, probeRadius, dir, out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
            targetDistance = hit.distance;

        targetDistance = Mathf.Clamp(targetDistance, minDistance, distance);

        if (targetDistance < currentDistance)
            currentDistance = targetDistance;
        else
            currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, recoverSpeed * Time.deltaTime);

        transform.position = followPoint + dir * currentDistance;
        transform.SetPositionAndRotation(followPoint + dir * currentDistance, controller.transform.rotation);
    }
}
