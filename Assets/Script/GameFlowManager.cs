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
    private WaveEffect waveEffect;

    [SerializeField]
    private ScoreUI scoreUI;
    [SerializeField]
    private ComboUI comboUI;
    [SerializeField]
    private SpawnerUI spawnerUI;
    [SerializeField]
    private PlayerUI playerUI;
    private EnemyPool enemyPool;


    private void Awake()
    {
        player.OnPlayerDead += GameOver;
        spawner.OnWaveClear += WaveClear;
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
        playerUI.Initialize(player);
        spawner.Initialize(player.transform, spawnerUI, comboManager);
        waveEffect.Initialize(spawner);
    }

    private void GameStart()
    {
        spawner.GameStart();
    }

    private void GameOver()
    {
        player.gameObject.SetActive(false);
        spawner.GameOver();
        gameOverUI.StartGameOverUI(comboManager.GetScore());
        Debug.Log("Game Over");
    }

    private void WaveClear(WaveInfo waveinfo)
    {
        player.PlayerHeal();
        waveEffect.OnWaveChanged(waveinfo);
        spawnerUI.UpdateWaveText(waveinfo.wave);
    }
}
