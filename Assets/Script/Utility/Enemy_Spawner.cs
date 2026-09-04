using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using UnityEditor.Overlays;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    [SerializeField]
    EnemyPool pool;

    [SerializeField]
    float spawnRangeX = 1f, spawnRangeY = 1f;
    [SerializeField]
    private float waveEffectDuration;
    [SerializeField]
    private GameObject spawnPointEffect;

    private void Awake()
    {
        pool = GetComponent<EnemyPool>();
        spawnPointEffect.SetActive(false);
    }

    public void Spawn(int index)
    {
        var enemyObj = pool.Pop(index);
        enemyObj.SetActive(true);
        enemyObj.transform.position = SpawnPoint;
    }

    public void EnableEffect()
    {
        StartCoroutine(WaveEffect());
    }

    IEnumerator WaveEffect()
    {
        spawnPointEffect.SetActive(true);
        yield return new WaitForSeconds(waveEffectDuration);
        spawnPointEffect.SetActive(false);
    }

    public Vector3 SpawnPoint => new Vector3(Random.Range(-spawnRangeX, spawnRangeX), 0, Random.Range(-spawnRangeY, spawnRangeY)) + transform.position;
}
