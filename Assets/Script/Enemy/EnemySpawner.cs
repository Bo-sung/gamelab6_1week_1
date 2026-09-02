using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

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
public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private float spawnInterval = 0.5f;

    [SerializeField]
    private float spawnRate = 10f;

    [SerializeField]
    private Transform[] spawnPoints;

    private ISpawnUI spawnerUI;

    private EnemyPool pool;

    // ���� wave
    public int currentWave = 1;

    // ���� ��ġ(��)
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
        if(spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("���� ����Ʈ ����");
            //SpawnFallBack();
        }
    }
    [ContextMenu("�ϴ� �������� 10�� �ڱ�")]
    private void SpawnFallBack()
    {
        GameObject prefab = new GameObject();    // ������ ������
        int count = 10;        // ������ ����
        float radius = 10f;
        spawnPoints = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            // 1. ���� ������ �´� ���� ��� (���� ����)
            float angle = i * Mathf.PI * 2 / count;

            // 2. �ﰢ�Լ��� X, Z (�Ǵ� Y) ��ǥ ���
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            // 3. ������ ��ġ ���� (��ũ��Ʈ�� ���� ������Ʈ �߽�)
            Vector3 spawnPosition = new Vector3(x, 0, z) + transform.position;

            // 4. ������Ʈ ���� �� �ٱ����� �ٶ󺸵��� ȸ�� ����
            Quaternion spawnRotation = Quaternion.LookRotation(spawnPosition - transform.position);
            var point = Instantiate(prefab, spawnPosition, spawnRotation,this.transform);
            point.name = $"SpawnPoint_{i}";
            spawnPoints[i] = point.transform;
        }
    }
    
    IEnumerator SpawnEnemy1()
    {
        int waveIndex = 0;
        int tableIndex = 0;

        while (isGameOn)
        {
            if (waveIndex >= waveInfos.Length)
            {
                // ���̺� �ٳ���
                continue;
            }
            var waveData = waveInfos[waveIndex];

            if (tableIndex >= waveData.enemySpawnTable.Length)
            {
                //���� ���̺� �Ѱ�

                yield return new WaitForSeconds(spawnRate);
                waveIndex++;
                tableIndex = 0;
                spawnerUI.UpdateWaveText(waveData.wave);
                continue;
            }
            var spawnArr = waveData.enemySpawnTable;
            // ���� ó��
            var enemyObj = pool.Pop(spawnArr[tableIndex]);
            enemyObj.SetActive(true);
            enemyObj.transform.position = ExtractSafeSpawnPoint(waveData).position;
            UnityEngine.Debug.Log($"EnemySpawn!! Name : {enemyObj.name}, Wave : {waveData.wave}, TableIndex : {tableIndex}, SpawnIndex : {spawnArr[tableIndex]}");
            // ���� ���͹� ���
            yield return new WaitForSeconds(spawnInterval);
            tableIndex++;
        }
    }

    private Transform ExtractSafeSpawnPoint(WaveInfo info)
    {
        // ���� ����Ʈ ���̺�
        var spawnPointTable = info.spawnPointTable;
        // ���̺� ����
        int tableLen = spawnPointTable.Length;

        // ���̺� �� ���� ����
        int randNum = UnityEngine.Random.Range(0, tableLen - 1);

        // ��������� �� ����
        int selectedIndex = spawnPointTable[randNum];

        // ���� ������� ����Ʈ �迭���� �����ϸ�
        if (selectedIndex >= spawnPoints.Length)
        {
            // ���� ����Ʈ�� ���� ���������� ���� ����
            selectedIndex = spawnPoints.Length - 1;
        }
        // ������� ���� ����Ʈ���� ����
        var result = spawnPoints[selectedIndex];

        Debug.Log($"Selected Spawn Point Index : {selectedIndex}");
        return result;
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
        new WaveInfo(0,new int[] { 0,0,0,0,0,0 },new int[] { 0,1,2,3}),
        new WaveInfo(1,new int[] { 1,0,1,0,1,0 },new int[] { 0,1,2,3,4}),
        new WaveInfo(2,new int[] { 1,0,1,0,1,0 },new int[] { 0,1,2,3,4,5}),
        new WaveInfo(3,new int[] { 1,0,1,0,1,0 },new int[] { 0,1,2,3,4,5,6}),
        new WaveInfo(4,new int[] { 1,0,1,0,1,0 },new int[] { 0,1,2,3,4,5,6,7 }),
        new WaveInfo(5,new int[] { 1,0,1,0,1,0 },new int[] { 0,2,4,6,8 }),
        new WaveInfo(6,new int[] { 1,0,1,0,1,0 },new int[] { 1,2,3,4,5,6,7 }),
        new WaveInfo(7,new int[] { 1,0,1,0,1,0 },new int[] { 4,5,6,7 }),
        new WaveInfo(8,new int[] { 1,0,1,0,1,0 },new int[] { 0,4,5,6,7 }),
        new WaveInfo(9,new int[] { 1,0,1,0,1,0 },new int[] { 2,3,4,5,6,7 }),

    };
}
