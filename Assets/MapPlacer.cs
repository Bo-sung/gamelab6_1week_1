using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject mapPrefab;
    [SerializeField]
    private List<Enemy_Spawner> spawner;
    [SerializeField]
    private List<GameObject> gos;

    [SerializeField]
    int WallCount = 150;
    [SerializeField]
    float MinimumDist = 5;
    [SerializeField]
    float MaximumDist = 10;

    int mapLayer = 3;
    int mapLayerIndex = 0;


    private void Start()
    {
    }

    [ContextMenu("테스트")]
    private void Spawn()
    {
        gos.Clear();
        var spawnerindex = 0;
        for (int i = 0; i < WallCount + spawner.Count; ++i)
        {
            int rand = Random.Range(0, 10);
            if (rand == 0 && spawnerindex < spawner.Count)
            {
                var randResult = GetRandDist();
                spawner[spawnerindex].transform.position = randResult.position;
                gos.Add(spawner[spawnerindex].gameObject);
                spawnerindex++;
            }
            else
            {
                var randResult = GetRandDist();
                var temp = Instantiate(mapPrefab, this.transform);
                temp.transform.position = randResult.position;
                temp.transform.localScale += Vector3.up * randResult.height;
                gos.Add(temp);
            }
        }
    }

    private (Vector3 position, float height) GetRandDist()
    {
        Vector3 pos;
        float height;
        pos = Random.onUnitSphere;
        // 레이어 넘어가는 조건 달성 여부 체크
        if (mapLayerIndex <= Math.Pow(3, mapLayer))
        {
            mapLayerIndex++;

            pos = pos * mapLayer * Mathf.Lerp(MinimumDist, MaximumDist, Random.Range(0f, 1f));
            height = Mathf.Abs(pos.y);
            pos.y = 0f;

            return (pos, height);
        }
        else
        {
            mapLayerIndex = 0;
            mapLayer++;

            pos = pos * mapLayer * Mathf.Lerp(MinimumDist, MaximumDist, Random.Range(0f, 1f));
            height = Mathf.Abs(pos.y);
            pos.y = 0f;

            return (pos, height);
        }
    }
}
