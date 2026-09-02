using System;
using System.Collections;
using UnityEngine;

public interface ISpawnUI
{
    void UpdateWaveText(int wave);
}

public struct WaveInfo
{
    public int wave;
    public int[] enemySpawnTable;

    public WaveInfo(int wave, int[] enemySpawnTable)
    {
        this.wave = wave;
        this.enemySpawnTable = enemySpawnTable;
    }
}


[RequireComponent(typeof(EnemyPool))]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private float spawnInterval = 0.5f;

    [SerializeField]
    private float spawnRate = 10f;

    private ISpawnUI spawnerUI;

    private EnemyPool pool;

    // 현재 wave
    public int currentWave = 1;

    // 스폰 위치(원)
    public float spawnDistance = 50f;

    private bool isGameOn;

    private void Awake()
    {
        pool = GetComponent<EnemyPool>();
    }

    public void Initialize(Transform target, ISpawnUI spawnUI, IScoreHandler scoreHandler)
    {
        spawnerUI = spawnUI;
        pool.Initialize(target, scoreHandler);
    }

    private void Start()
    {
        GameStart();
    }

    IEnumerator SpawnEnemy1()
    {
        int waveIndex = 0;
        int tableIndex = 0;

        while (isGameOn)
        {
            Vector3 center = transform.position; // 0, 0 중심
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * spawnDistance;
            Vector3 randomPos;

            // 현재 wave에 따라서 제한 조건이 풀리는 식으로
            if (randomCircle.x >= 0 && randomCircle.y >= 0)
            {
                randomPos = new Vector3(randomCircle.x, 0, randomCircle.y); // 0, 0 중심이므로 구한 값으로만 좌표 설정
            }
            else if (randomCircle.x >= 0 && randomCircle.y <= 0 && currentWave >= 5)
            {
                randomPos = new Vector3(randomCircle.x, 0, randomCircle.y);
            }
            else if (randomCircle.x <= 0 && randomCircle.y >= 0 && currentWave >= 10)
            {
                randomPos = new Vector3(randomCircle.x, 0, randomCircle.y);
            }
            else if (randomCircle.x <= 0 && randomCircle.y <= 0 && currentWave >= 15)
            {
                randomPos = new Vector3(randomCircle.x, 0, randomCircle.y);
            }
            else
            {
                continue;
            }

            if (waveIndex >= waveInfos.Length)
            {
                // 웨이브 다끝남
                continue;
            }
            var waveData = waveInfos[waveIndex];

            if (tableIndex >= waveData.enemySpawnTable.Length)
            {
                //다음 웨이브 넘겨

                yield return new WaitForSeconds(spawnRate);
                waveIndex++;
                spawnerUI.UpdateWaveText(waveData.wave);
                continue;
            }
            var spawnArr = waveData.enemySpawnTable;

            // 스폰 처리
            var enemyObj = pool.Pop(spawnArr[tableIndex++]);
            enemyObj.SetActive(true);
            enemyObj.transform.position = randomPos;
            // 스폰 인터벌 대기
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    public void GameStart()
    {
        isGameOn = true;
        StartCoroutine(SpawnEnemy1());
    }

    public void GameOver()
    {
        isGameOn = false;
    }

    private WaveInfo[] waveInfos = new WaveInfo[]
    {
        new WaveInfo(0,new int[] { 0,0,0,0,0,0 }),
        new WaveInfo(1,new int[] { 1,0,1,0,1,0 }),
        new WaveInfo(2,new int[] { 1,0,1,0,1,0 }),
        new WaveInfo(3,new int[] { 1,0,1,0,1,0 }),
        new WaveInfo(4,new int[] { 1,0,1,0,1,0 }),
        new WaveInfo(5,new int[] { 1,0,1,0,1,0 }),
        new WaveInfo(6,new int[] { 1,0,1,0,1,0 }),
        new WaveInfo(7,new int[] { 1,0,1,0,1,0 }),
        new WaveInfo(8,new int[] { 1,0,1,0,1,0 }),
        new WaveInfo(9,new int[] { 1,0,1,0,1,0 }),

    };
}
