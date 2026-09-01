using UnityEngine;

public class RotateWall : MonoBehaviour
{
    public bool isRight = true;
    public float rotationSpeed = 50f;
    void Update()
    {
        if(isRight)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
    }
}
