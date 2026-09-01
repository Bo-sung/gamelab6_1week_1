using System.Collections.Generic;
using UnityEngine;


public class EnemyPool : MonoBehaviour
{
    public static EnemyPool instance;

    [SerializeField] private GameObject enemyObject;
    [SerializeField] private GameObject enemyObject2;
    [SerializeField] private int poolSize;
 
    private List<GameObject> pool;
    private List<GameObject> pool2;

    public float minSpeed = 0.01f;
    public float maxSpeed = 0.05f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        Initialize();
    }

    private void Initialize()
    {
        pool = new List<GameObject>();
        for(int i = 0; i < poolSize; i++)
        {
            var enemy =  Instantiate(enemyObject);
            enemy.SetActive(false);
            enemy.GetComponent<Enemy>().speed = Random.Range(minSpeed, maxSpeed);
            pool.Add(enemy);
        }
        pool2 = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            var enemy = Instantiate(enemyObject2);
            enemy.SetActive(false);
            enemy.GetComponent<Enemy2>().speed = Random.Range(minSpeed, maxSpeed);
            pool2.Add(enemy);
        }
    }

    public GameObject GetObject() 
    {
        foreach(var enemy in pool)
        {
            if(!enemy.activeInHierarchy) return enemy;  
        }

        var e = Instantiate(enemyObject);
        e.SetActive(false);
        pool.Add(e);

        return e;
       
    }

    public GameObject GetObject2()
    {
        foreach (var enemy in pool2)
        {
            if (!enemy.activeInHierarchy) return enemy;
        }
        var e = Instantiate(enemyObject2);
        e.SetActive(false);
        pool2.Add(e);
        return e;
    }

}
