using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 100f;

    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private CarManager carManager;
    void Start()
    {
        inputManager.OnAPressed += TurnLeft;
        inputManager.OnDPressed += TurnRight;
        inputManager.OnWPressed += MoveForward;
        inputManager.OnSPressed += MoveBackward;
    }
    void OnDestroy()
    {
        inputManager.OnAPressed -= TurnLeft;
        inputManager.OnDPressed -= TurnRight;
        inputManager.OnWPressed -= MoveForward;
        inputManager.OnSPressed -= MoveBackward;
    }

    void TurnLeft()
    {
        carManager.Steer(-1);
    }
    void TurnRight()
    {
        carManager.Steer(1);
    }
    void MoveForward()
    {
        carManager.Accelerate(1);
    }
    void MoveBackward()
    {
        carManager.Accelerate(-1);
    }
}

// 에러 방지용 임시 CarManager 클래스
class CarManager
{
    public void Steer(int direction)
    {
        // Implement steering logic here
        Debug.Log($"Steering in direction: {direction}");
    }

    public void Accelerate(int direction)
    {
        // Implement acceleration logic here
        Debug.Log($"Accelerating in direction: {direction}");
    }
}
