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
    public int nextWaveEnemy2Num = 0;
    // wave마다 요구되는 적의 수 
    public int requireEnemyNum = 3;
    public int requireEnemy2Num = 0;


    // 스폰 위치(원)
    public float spawnDistance = 50f;

    private bool isGameOn;

    private void Start()
    {
        nextWaveEnemyNum = requireEnemyNum;
        GameStart();
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

            if (requireEnemyNum != 0 && requireEnemy2Num != 0)
            {
                if (Random.Range(0, 2) == 0)
                {
                    var enemyObj = EnemyPool.instance.GetObject();
                    enemyObj.SetActive(true);
                    enemyObj.transform.position = randomPos;
                    requireEnemyNum--;
                }
                else
                {
                    var enemy2Obj = EnemyPool.instance.GetObject2();
                    enemy2Obj.SetActive(true);
                    enemy2Obj.transform.position = randomPos;
                    requireEnemy2Num--;
                }
            }
            else if (requireEnemy2Num <= 0)
            {
                var enemyObj = EnemyPool.instance.GetObject();
                enemyObj.SetActive(true);
                enemyObj.transform.position = randomPos;

                requireEnemyNum--;
            } 
            else
            {
                var enemy2Obj = EnemyPool.instance.GetObject2();
                enemy2Obj.SetActive(true);
                enemy2Obj.transform.position = randomPos;

                requireEnemy2Num--;
            }

            if(requireEnemyNum <= 0 && requireEnemy2Num <= 0)
            {
                if (currentWave % 4 == 0)
                {
                    nextWaveEnemy2Num++;
                } 
                else
                {
                    nextWaveEnemyNum++;
                }
                requireEnemy2Num = nextWaveEnemy2Num;
                requireEnemyNum = nextWaveEnemyNum;
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
