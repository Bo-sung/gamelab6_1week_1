using UnityEngine;
[DisallowMultipleComponent]
public class CarStat : MonoBehaviour
{
    [SerializeField]    
    private float speed = 10f;
    [SerializeField]
    private float acceleration = 5f;
    [SerializeField]
    private float braking = 8f;
    [SerializeField]
    private float cornering = 7f;
    [SerializeField]
    private float weight = 1000f;

    public float Speed { get => speed; private set => speed = value; }
    public float Acceleration { get => acceleration; private set => acceleration = value; }
    public float Braking { get => braking; private set => braking = value; }
    public float Cornering { get => cornering; private set => cornering = value; }
    public float Weight { get => weight; private set => weight = value; }


    public void UpdateStat(float speed, float acceleration, float braking, float cornering, float weight)
    {
        this.speed = speed;
        this.acceleration = acceleration;
        this.braking = braking;
        this.cornering = cornering;
        this.weight = weight;
    }
}