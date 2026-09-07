using UnityEngine;

public class AimCam : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    Transform target;
    [SerializeField]
    private Vector3 positionGap;
    [SerializeField]
    float verticalSmoothTime = 0.12f;

    [Header("Orbit")]
    [SerializeField]
    float distance = 7f;
    [SerializeField]
    float minDistance = 1.2f;

    [Header("FOV")]
    [SerializeField]
    float normalFov = 60f;

    Camera cam;
    void Awake() => cam = GetComponent<Camera>();

    float currentDistance;
    Vector3 followPoint;
    float xVelocity;
    float yVelocity;
    float zVelocity;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        currentDistance = distance;
        followPoint = target.transform.position;
    }

    void LateUpdate()
    {
        HandleCam();
    }

    private void OnEnable()
    {
        followPoint = target.position;
    }

    private void HandleCam()
    {
        // 카메라 댐핑
        followPoint.x = Mathf.SmoothDamp(followPoint.x, target.position.x + positionGap.x, ref xVelocity, verticalSmoothTime);
        followPoint.z = Mathf.SmoothDamp(followPoint.z, target.position.z + positionGap.z, ref zVelocity, verticalSmoothTime);
        followPoint.y = Mathf.SmoothDamp(followPoint.y, target.position.y + positionGap.y, ref yVelocity, verticalSmoothTime);
        var dir = target.transform.rotation * Vector3.back;

        transform.position = followPoint + dir * currentDistance;
        transform.SetPositionAndRotation(followPoint + dir * currentDistance, target.transform.rotation);
    }
}