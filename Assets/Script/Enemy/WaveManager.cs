using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameScene;
using Random = UnityEngine.Random;

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
    private float waveInterval = 20f;

    [SerializeField]
    private float spawnInterval = 0.5f;

    [SerializeField]
    private float spawnRate = 10f;

    [SerializeField]
    public Enemy_Spawner[] spawnPoints;

    public System.Action<WaveInfo> OnWaveClear;
    public System.Action OnWaveStart;
    public System.Action OnWaveEmpty;

    private EnemyPool pool;

    public WaveInfo[] WaveInfos => waveInfos;

    private void Awake()
    {
        pool = GetComponent<EnemyPool>();

        foreach (var item in spawnPoints)
        {
            item.Initialize(pool);
        }
    }

    public void Initialize(Transform target, IScoreHandler scoreHandler, IMiniMap miniMapUI)
    {
        pool.Initialize(target, scoreHandler, miniMapUI);
    }

    Queue<(int pointIndex, int spawnIndex)> spawnQueue = new Queue<(int pointIndex, int spawnIndex)>();
    int waveCount = 0;
    void Wave(WaveInfo info)
    {
        int wave = info.wave;
        foreach (var enemyIndex in info.enemySpawnTable)
        {
            (int pointIndex, int spawnIndex) queueData;
            var pindex = Random.Range(0, info.spawnPointTable.Length - 1);
            queueData.pointIndex = info.spawnPointTable[pindex];
            queueData.spawnIndex = enemyIndex;
            // 스폰 포인트 이펙트 실행
            spawnPoints[queueData.pointIndex].EnableEffect();

            spawnQueue.Enqueue(queueData);
        }
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        yield return new WaitForSeconds(waveInterval);
        OnWaveStart?.Invoke();
        while (spawnQueue.Count != 0)
        {
            var spawnInfo = spawnQueue.Dequeue();
            spawnPoints[spawnInfo.pointIndex].Spawn(spawnInfo.spawnIndex);
            yield return new WaitForSeconds(spawnInterval);
        }

        if (waveCount >= WaveInfos.Length)
            OnWaveEmpty?.Invoke();
        else
            OnWaveClear?.Invoke(waveInfos[waveCount++]);
    }

    public void StartGame()
    {
        Debug.Log("Game Start");
        Wave(waveInfos[0]);
    }

    public void NextWave(WaveInfo wave)
    {
        Wave(wave);
    }

    private WaveInfo[] waveInfos = new WaveInfo[]
    {
        new WaveInfo(1,new int[] { 0,1,0,1,0,1 },new int[] { 0,1,2,3}),
        new WaveInfo(2,new int[] { 0,0,0,0,1,0,1,1 },new int[] { 0,1,2,3,4}),
        new WaveInfo(3,new int[] { 1,0,1,2,1,0 },new int[] { 0,1,2,3,4,5}),
        new WaveInfo(4,new int[] { 0,2,1,0,2,2 },new int[] { 0,1,2,3,4,5,6}),
        new WaveInfo(5,new int[] { 1,2,3,0,1,0,2,0 },new int[] { 0,1,2,3,4,5,6,7 }),
        new WaveInfo(6,new int[] { 1,2,3,3,3,0,2,0 },new int[] { 0,2,4,6,7 }),
        new WaveInfo(7,new int[] { 1,2,0,0,3,1 },new int[] { 1,2,3,4,5,6,7 }),
        new WaveInfo(8,new int[] { 2,3,1,1,0,1,0,1,1,2,0 },new int[] { 4,5,6,7 }),
        new WaveInfo(9,new int[] { 3,2,3,1,0,1,0,3,1,2,0,1,1,2,0 },new int[] { 0,4,5,6,7 }),
        new WaveInfo(10,new int[] { 1,3, 1, 0, 2, 3, 2,3,1, 2, 0, 1, 1, 1,0,3,1,2,0 },new int[] { 0,1,2,3,4,5,6,7 }),
        new WaveInfo(11,new int[] { 2,3,1,1,0,1,0,1,1,2,0 },new int[] { 4,5,6,7 }),
        new WaveInfo(12,new int[] { 3,2,3,1,0,1,0,3,1,2,0,1,1,2,0 },new int[] { 0,4,5,6,7 }),
        new WaveInfo(13,new int[] { 1,3, 1, 0, 2, 3, 2,3,1, 2, 0, 1, 1, 1,0,3,1,2,0 },new int[] { 0,1,2,3,4,5,6,7 }),
        new WaveInfo(14,new int[] { 2,3,1,1,0,1,0,1,1,2,0 },new int[] { 4,5,6,7 }),
        new WaveInfo(15,new int[] { 3,2,3,1,0,1,0,3,1,2,0,1,1,2,0 },new int[] { 0,4,5,6,7 }),
        new WaveInfo(16,new int[] { 1,3, 1, 0, 2, 3, 2,3,1, 2, 0, 1, 1, 1,0,3,1,2,0 },new int[] { 0,1,2,3,4,5,6,7 }),
        new WaveInfo(17,new int[] { 2,3,1,1,0,1,0,1,1,2,0 },new int[] { 4,5,6,7 }),
        new WaveInfo(18,new int[] { 3,2,3,1,0,1,0,3,1,2,0,1,1,2,0 },new int[] { 0,4,5,6,7 }),
        new WaveInfo(19,new int[] { 1,3, 1, 0, 2, 3, 2,3,1, 2, 0, 1, 1, 1,0,3,1,2,0 },new int[] { 0,1,2,3,4,5,6,7 }),
        new WaveInfo(20,new int[] { 1,3, 1, 0, 2, 3, 2,3,1, 2, 0, 1, 1, 1,0,3,1,2,0 },new int[] { 0,1,2,3,4,5,6,7 }),
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
    /// <param name="compoList"></param>
    private void GetFromChild_recur<T>(Transform transform, List<T> compoList, int depth) where T : MonoBehaviour
    {
        // 뎁스 0 = 충분히 깊다 그만
        if (depth == 0)
            return;
        var compo = transform.GetComponent<T>();
        if (compo != null)
        {
            compoList.Add(compo);
            return;
        }
        for (int i = 0; i < transform.childCount; ++i)
        {
            var child = transform.GetChild(i);
            GetFromChild_recur<T>(child, compoList, depth - 1);
        }
    }
}
