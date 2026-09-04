
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public interface ISpawnUI
{
    void UpdateWaveText(int wave);
}

public struct WaveInfo
{
    public int wave;
    public int[] enemySpawnTable;

    public int[] spawnPointTable;

    public WaveInfo(int wave, int[] enemySpawnTable, int[] spawnPointTable)
    {
        this.wave = wave;
        this.enemySpawnTable = enemySpawnTable;
        this.spawnPointTable = spawnPointTable;
    }
}


[RequireComponent(typeof(EnemyPool))]
public class WaveManager : MonoBehaviour
{
    [SerializeField]
    private float waveInterval = 10f;

    [SerializeField]
    private float spawnInterval = 0.5f;

    [SerializeField]
    private float spawnRate = 10f;

    [SerializeField]
    public Enemy_Spawner[] spawnPoints;

    public System.Action<WaveInfo> OnWaveClear;
    public System.Action OnWaveStart;

    private EnemyPool pool;

    public WaveInfo[] WaveInfos => waveInfos;

    private void Awake()
    {
        pool = GetComponent<EnemyPool>();
    }

    public void Initialize(Transform target, ISpawnUI spawnUI, IScoreHandler scoreHandler)
    {
        pool.Initialize(target, scoreHandler);
    }

    Queue<(int pointIndex, int spawnIndex)> spawnQueue;
    int waveCount = 0;
    void Wave(WaveInfo info)
    {
        int wave = info.wave;
        foreach (var pointIndex in info.spawnPointTable)
        {
            (int pointIndex, int spawnIndex) queueData;
            queueData.spawnIndex = Random.Range(0, info.enemySpawnTable.Length);
            queueData.pointIndex = pointIndex;
            // 스폰 포인트 이펙트 실행
            spawnPoints[pointIndex].EnableEffect();

            spawnQueue.Enqueue(queueData);
        }
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        OnWaveStart?.Invoke();
        while (spawnQueue.Count != 0)
        {
            var spawnInfo = spawnQueue.Dequeue();
            spawnPoints[spawnInfo.pointIndex].Spawn(spawnInfo.spawnIndex);
            yield return new WaitForSeconds(spawnInterval);
        }

        OnWaveClear?.Invoke(waveInfos[waveCount++]);
    }

    public void GameStart()
    {
        Wave(waveInfos[0]);
    }

    public void GameOver()
    {

    }

    private WaveInfo[] waveInfos = new WaveInfo[]
    {
        new WaveInfo(1,new int[] { 0,1,0,1,0,1 },new int[] { 0,1,2,3}),
        new WaveInfo(2,new int[] { 0,0,0,0,1,0,1,1 },new int[] { 0,1,2,3,4}),
        new WaveInfo(3,new int[] { 1,0,1,2,1,0 },new int[] { 0,1,2,3,4,5}),
        new WaveInfo(4,new int[] { 0,2,1,0,2,2 },new int[] { 0,1,2,3,4,5,6}),
        new WaveInfo(5,new int[] { 1,2,3,0,1,0,2,0 },new int[] { 0,1,2,3,4,5,6,7 }),
        new WaveInfo(6,new int[] { 1,2,3,3,3,0,2,0 },new int[] { 0,2,4,6,8 }),
        new WaveInfo(7,new int[] { 1,2,0,0,3,1 },new int[] { 1,2,3,4,5,6,7 }),
        new WaveInfo(8,new int[] { 2,3,1,1,0,1,0,1,1,2,0 },new int[] { 4,5,6,7 }),
        new WaveInfo(9,new int[] { 3,2,3,1,0,1,0,3,1,2,0,1,1,2,0 },new int[] { 0,4,5,6,7 }),
        new WaveInfo(10,new int[] { 1,3, 1, 0, 2, 3, 2,3,1, 2, 0, 1, 1, 1,0,3,1,2,0 },new int[] { 0,1,2,3,4,5,6,7 }),
    };

    [ContextMenu("자식 찾기")]
    private void AutoFind()
    {
        List<Enemy_Spawner> tempList = new List<Enemy_Spawner>();
        GetFromChild_recur<Enemy_Spawner>(this.transform, tempList, 10);
        spawnPoints = tempList.ToArray();
    }

    /// <summary>
    /// 자식들 재귀 순회 하면서 컴포넌트 전부 찾는 코드. 단 자신이 찾는 컴포넌트면 스톱, 다만 depth 값이 0이 되면 스톱(제한)
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="tempList"></param>
    private void GetFromChild_recur<T>(Transform transform, List<T> tempList, int depth) where T : MonoBehaviour
    {
        // 뎁스 0 = 충분히 깊다 그만
        if (depth == 0)
            return;
        var spawner = transform.GetComponent<T>();
        if (spawner != null)
        {
            tempList.Add(spawner);
            return;
        }
        for (int i = 0; i < transform.childCount; ++i)
        {
            var child = transform.GetChild(i);
            GetFromChild_recur<T>(child, tempList, depth - 1);
        }
    }
}
