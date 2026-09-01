using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField]
    private float spawnInterval = 0.5f;

    [SerializeField]
    private float spawnRate = 10f;

    // 현재 wave
    public int currentWave = 1;

    // 다음 wave에 요구되는 적의 수
    public int nextWaveEnemyNum = 0;
    // wave마다 요구되는 적의 수 
    public int requireEnemyNum = 3;

    // 스폰 위치(원)
    private float spawnDistance = 10f;

    private bool isGameOn;

    private void Start()
    {
        GameStart();

        nextWaveEnemyNum = requireEnemyNum + 1;
    }

    IEnumerator SpawnEnemy()
    {
        while (isGameOn)
        {
            Vector3 center = transform.position; // 0, 0 중심

            Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnDistance;

            Vector3 randomPos;

            // 현재 wave에 따라서 제한 조건이 풀리는 식으로
            if(randomCircle.x >= 0 && randomCircle.y >= 0)
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

            var enemyObj = EnemyPool.instance.GetObject();
            enemyObj.SetActive(true);
            enemyObj.transform.position = randomPos;

            requireEnemyNum--;

            if(requireEnemyNum <= 0)
            {
                requireEnemyNum = nextWaveEnemyNum;
                nextWaveEnemyNum ++;
                currentWave++;

                yield return new WaitForSeconds(spawnRate);
            }
            else
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        
    }

    public void GameStart()
    {
        isGameOn = true;
        StartCoroutine(SpawnEnemy());
    }

    public void GameOver()
    {
        isGameOn = false;
    }
}
