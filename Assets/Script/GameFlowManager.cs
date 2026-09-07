using System;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public Player player;
    public GameObject arrow;
    public GameOverUI gameOverUI;
    public GameOverUI gameClearUI;

    [SerializeField]
    private ArrowController controller;
    [SerializeField]
    private ComboManager comboManager;

    [SerializeField]
    private WaveManager spawner;

    [SerializeField]
    private ScoreUI scoreUI;
    [SerializeField]
    private ComboUI comboUI;
    [SerializeField]
    private SpawnerUI spawnerUI;
    [SerializeField]
    private PlayerUI playerUI;
    private EnemyPool enemyPool;


    private int currentWave = 0;

    private void Awake()
    {
        //player.OnPlayerDead += GameOver;
        //spawner.OnWaveClear += WaveClear;
        //spawner.OnGameClear += GameClear;
        //Initialize();
    }

    private void Start()
    {

        //GameStart();
    }

    private void Initialize()
    {
        comboManager.Initialize(controller);
        scoreUI.Initialize(comboManager);
        comboUI.Initialize(comboManager);
    }

    private void GameStart()
    {
    }

    private void GameClear()
    {
        player.gameObject.SetActive(false);
        arrow.SetActive(false);
        gameClearUI.StartGameOverUI(comboManager.GetScore() + 100*currentWave);
        Cursor.lockState = CursorLockMode.None; // 화면 중앙에 고정
        Cursor.visible = true;
    }

    private void GameOver()
    {
        player.gameObject.SetActive(false);
        arrow.SetActive(false);
        gameOverUI.StartGameOverUI(comboManager.GetScore() + 100 * currentWave);
        Cursor.lockState = CursorLockMode.None; // 화면 중앙에 고정
        Cursor.visible = true;
        Debug.Log("Game Over");
    }

    private void WaveClear(WaveInfo waveinfo)
    {
        //player.PlayerHeal();
        //waveEffect.OnWaveChanged(waveinfo);
        spawnerUI.UpdateWaveText(waveinfo.wave);

        currentWave = waveinfo.wave;
    }
}
