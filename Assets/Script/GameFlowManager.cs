using System;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public Player player;
    public GameOverUI gameOverUI;

    [SerializeField]
    private ArrowController controller;
    [SerializeField]
    private ComboManager comboManager;

    [SerializeField]
    private EnemySpawner spawner;

    [SerializeField]
    private ScoreUI scoreUI;
    [SerializeField]
    private ComboUI comboUI;
    [SerializeField]
    private SpawnerUI spawnerUI;
    private EnemyPool enemyPool;


    private void Awake()
    {
        player.OnPlayerDead += GameOver;
        Initialize();
    }

    private void Start()
    {

        GameStart();
    }

    private void Initialize()
    {
        comboManager.Initialize(controller);
        scoreUI.Initialize(comboManager);
        comboUI.Initialize(comboManager);
        spawner.Initialize(player.transform, spawnerUI, comboManager);
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
