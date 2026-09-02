using System;
using System.Collections.Generic;
using UnityEngine;


public class EnemyPool : MonoBehaviour
{
    [SerializeField] private List<EnemyBase> enemyPrefabs;
    [SerializeField] private int poolSize;

    private List<List<GameObject>> pools;

    private Transform target;
    private IScoreHandler scoreHandler;

    public float minSpeed = 0.01f;
    public float maxSpeed = 0.05f;

    public void Initialize(Transform target, IScoreHandler scoreHandler)
    {
        this.target = target;
        this.scoreHandler = scoreHandler;

        pools = new List<List<GameObject>>();
        foreach (var prefab in enemyPrefabs)
        {
            var pool = new List<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                pool.Add(SpawnEnemy(prefab));
            }
            pools.Add(pool);
        }
    }

    private GameObject SpawnEnemy(EnemyBase enemyBase)
    {
        var prefab = Instantiate(enemyBase.gameObject);
        prefab.SetActive(false);
        if (enemyBase.GetType() == typeof(Enemy))
        {
            Enemy enemy = prefab.GetComponent<Enemy>();
            float speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
            enemy.SetData(target, scoreHandler, speed);
        }
        else if (enemyBase.GetType() == typeof(Enemy2))
        {
            Enemy2 enemy = prefab.GetComponent<Enemy2>();
            float speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
            enemy.SetData(target, scoreHandler, speed, 2);
        }
        return prefab;
    }

    public GameObject Pop(int index)
    {
        if (index >= pools.Count)
            return null;
        var selectedPool = pools[index];
        foreach (var item in selectedPool)
        {
            if (!item.activeInHierarchy)
                return item;
        }

        var newbie = SpawnEnemy(enemyPrefabs[index]);
        newbie.SetActive(false);
        selectedPool.Add(newbie);
        return newbie;
    }

    public GameObject GetObject()
    {
        return Pop(0);
    }

    public GameObject GetObject2()
    {
        return Pop(1);
    }
}
