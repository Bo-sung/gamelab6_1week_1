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
