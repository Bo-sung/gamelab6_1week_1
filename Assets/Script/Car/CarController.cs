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
        Initialize(null);
    }
    public void Initialize(CarStat stat)
    {
        carManager.Initialize(stat);
    }

    private void FixedUpdate()
    {
        carManager.Accelerate(Input.GetAxis("Vertical"));
        carManager.Steer(Input.GetAxis("Horizontal"));
        carManager.Brake(Input.GetKey(KeyCode.Space));
        carManager.AdjustStauts();
        
    }
}

