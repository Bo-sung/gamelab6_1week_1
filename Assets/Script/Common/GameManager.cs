using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    AugmentManager agManager;
    [SerializeField]
    CarStat carStat;

    private void Awake()
    {
        if (agManager == null)
        {
            agManager = GetComponent<AugmentManager>();
        }
        agManager.OnSelectAugment += HandleAugmentSelected;
    }

    private void HandleAugmentSelected(int obj)
    {
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
public class AugmentManager : MonoBehaviour
{
    public System.Action<int> OnSelectAugment;
    public void OpenAugmentPanel()
    {

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
}