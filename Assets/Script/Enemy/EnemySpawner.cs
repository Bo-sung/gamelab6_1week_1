using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField]
    private float SpawnInterval = 0.5f;
    [SerializeField]
    private Transform[] SpawnPoints;

    private bool isGameOn;

    private void Start()
    {
        StartGame();
    }
    IEnumerator SpawnEnemy()
    {
        while (isGameOn)
        {
            var enemyObj = EnemyPool.instance.GetObject();
            var point = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
            enemyObj.SetActive(true);
            enemyObj.transform.position = point.position;
            yield return new WaitForSeconds(SpawnInterval);
        }
        
    }

    public void StartGame()
    {
        isGameOn = true;
        StartCoroutine(SpawnEnemy());
    }

    public void EndGame()
    {
        isGameOn = false;
    }
}
