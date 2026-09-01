using System;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public Player player;
    public GameOverUI gameOverUI;

    [SerializeField]
    private EnemySpawner spawner;


    private void Awake()
    {
        player.OnPlayerDead += GameOver;
    }

    private void Initialize()
    {
    
    }

    private void GameStart()
    {
        spawner.GameStart();
    }

    private void GameOver()
    {
        player.gameObject.SetActive(false);
        spawner.GameOver();
        Debug.Log("Game Over");
    }
}
