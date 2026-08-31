using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    AugmentManger agManager;
    [SerializeField]
    CarStat carStat;

    private void Awake()
    {
        if (agManager == null)
        {
            agManager = GetComponent<AugmentManger>();
        }
        agManager.OnSelectAugment += HandleAugmentSelected;

    }

    private void HandleAugmentSelected(Augment data)
    {
        switch (data.type)
        {
            case AugmentType.SpeedUp:
                carStat.UpdateSpeed(carStat.Speed + data.value);
                Debug.Log("SpeedUp " + data.value);
                break;
            case AugmentType.AccelerationUp:
                carStat .UpdateAcceleration(carStat.Acceleration + data.value);
                Debug.Log("AccelerationUp " + data.value);
                break;
            case AugmentType.BrakeUp:
                carStat.UdpateBraking(carStat.Braking + data.value);
                Debug.Log("BrakeUp " + data.value);
                break;
            case AugmentType.CorneringUp:
                carStat.UpdateCornering(carStat.Cornering + data.value);
                Debug.Log("CorneringUp " + data.value);
                break;
            case AugmentType.WeightUp:
                carStat.UpdateWeight(carStat.Weight + data.value);
                Debug.Log("WeightUp " + data.value);
                break;
            case AugmentType.LoseWheel: 
                Debug.Log("LoseWheel");
                break;
            case AugmentType.ThrowBreak:
                Debug.Log("ThrowBreak");
                break;
            case AugmentType.SpringBumper:
                Debug.Log("SpringBumper");
                break;
        }
        throw new NotImplementedException();
    }

    public void StartGame()
    {
        // Implement game start logic here
        Console.WriteLine("Game Started");
    }
    public void EndGame()
    {
        // Implement game end logic here
        Console.WriteLine("Game Ended");
    }
}

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

    public void UpdateSpeed(float speed)
    {
        this.speed = speed;
    }
    public void UpdateAcceleration(float acceleration)
    {
        this.acceleration = acceleration;
    }

    public void UdpateBraking(float braking)
    {
        this.braking = braking;
    }
    public void UpdateCornering(float cornering)
    {
        this.cornering = cornering;
    }
    public void UpdateWeight(float weight)
    {
        this.weight = weight;
    }
}
