using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 100f;


    [SerializeField]
    private CarManager carManager;

    private void Start()
    {
        // 추후 Start를 제거하고 외부에서 초기화
        Initialize();
    }
    public void Initialize()
    {
        carManager.Initialize();
    }

    private void FixedUpdate()
    {
        carManager.Accelerate(Input.GetAxis("Vertical"));
        carManager.Steer(Input.GetAxis("Horizontal"));
        carManager.Brake(Input.GetKey(KeyCode.Space));
        carManager.AdjustStauts();
        
    }
}

// 에러 방지용 임시 CarManager 클래스
//class CarManager
//{
//    public void Steer(int direction)
//    {
//        // Implement steering logic here
//        Debug.Log($"Steering in direction: {direction}");
//    }

//    public void Accelerate(int direction)
//    {
//        // Implement acceleration logic here
//        Debug.Log($"Accelerating in direction: {direction}");
//    }
//}
