using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] float sensitivity = 3f;
    [SerializeField] float minPitch = -20f, maxPitch = 70f;
    [SerializeField] float speed = 2f;
    [SerializeField] float dashSpeed = 4f;

    float yaw, pitch = 28f;
    bool IsDashing => Input.GetKey(KeyCode.LeftShift);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        transform.Translate(Vector3.forward * Time.deltaTime * (IsDashing ? dashSpeed : speed));
        transform.rotation = rot;
        Debug.Log($"Yaw: {yaw}, Pitch: {pitch}");
    }
}
