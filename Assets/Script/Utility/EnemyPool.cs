using System.Collections.Generic;
using UnityEngine;


public class EnemyPool : MonoBehaviour
{
    public static EnemyPool instance;

    [SerializeField] private GameObject enemyObject;
    [SerializeField] private int poolSize;
 
    private List<GameObject> pool;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Initialize()
    {
        pool = new List<GameObject>();
        for(int i = 0; i < poolSize; i++)
        {
            var enemy =  Instantiate(enemyObject);
            enemy.SetActive(false);
            pool.Add(enemy);
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

}
