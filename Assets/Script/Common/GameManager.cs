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
    [SerializeField]
    TrackManager trackManager;

    private void Awake()
    {
        if (agManager == null)
        {
            agManager = GetComponent<AugmentManger>();
        }
        if (trackManager == null)
        {
            trackManager = GetComponent<TrackManager>();
        }
        agManager.OnSelectAugment += HandleAugmentSelected;
        trackManager.OnCarTrackFinish += HandleCarTrackFinish;
    }

    private void HandleAugmentSelected(Augment data)
    {
        switch (data.type)
        {
            case AugmentType.SpeedUp:
                carStat.ApplySpeed(data.value);
                Debug.Log("SpeedUp " + data.value);
                break;
            case AugmentType.AccelerationUp:
                carStat.ApplyAcceleration(data.value);
                Debug.Log("AccelerationUp " + data.value);
                break;
            case AugmentType.BrakeUp:
                carStat.ApplyBraking(data.value);
                Debug.Log("BrakeUp " + data.value);
                break;
            case AugmentType.CorneringUp:
                carStat.ApplyCornering(data.value);
                Debug.Log("CorneringUp " + data.value);
                break;
            case AugmentType.WeightUp:
                carStat.ApplyWeight(data.value);
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
    }

    private void HandleCarTrackFinish()
    {
        agManager.ActiveScreen();
    }

    public void StartGame()
    {
        Console.WriteLine("Game Started");
    }
    public void EndGame()
    {
        Console.WriteLine("Game Ended");
    }
}
