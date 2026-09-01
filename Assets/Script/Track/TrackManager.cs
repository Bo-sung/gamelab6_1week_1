using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public GameObject[] trackPrefabs;

    public List<TrackElement> generatedTracks;

    public TrackEdge trackEdge;

    private int currentTrackCount = 0;

    public System.Action OnCarTrackFinish;

    private void Awake()
    {
        trackEdge.OnTrigger += OnTriggerEdge;
    }

    private void OnTriggerEdge()
    {
        currentTrackCount++;
        OnCarTrackFinish?.Invoke();
    }

    public void Initialize()
    {
        // 초기 트랙 3개 생성
        for(int i=0; i<2; i++)
        {
            Generate();
        }
    }

    [ContextMenu("GenerateTrack")]
    public void Generate()
    {
        // OnCarTrackFinish > GameManager의 Evenet > Generate()
        // 트랙 생성

        // trackElement의 StartPoint에 Edge가 이동
        // 첫 트랙 없으면 여기서 널 발생하기 때문에 무조건 시작 지점은 세팅해두어야함.

        if(generatedTracks.Count == 0)
        {
            Debug.LogError("트랙이 생성되지 않았습니다. 초기 트랙을 생성해주세요.");
        }

        if(generatedTracks[currentTrackCount].endPoint == null)
        {
            Debug.LogError("트랙 엔드포인트 에러");
        }

        trackEdge.transform.position = generatedTracks[currentTrackCount].endPoint.position;

        // trackPrefab의 중심이 startPoint라는 가정 하에
        GameObject originTrack = trackPrefabs[Random.Range(0, trackPrefabs.Length - 1)];
        if (generatedTracks[generatedTracks.Count - 1].endPoint == null)
        {
            Debug.LogError("트랙 프리팹에러.");
            return;
        }
        Vector3 trackPosition = transform.TransformPoint(generatedTracks[generatedTracks.Count - 1].endPoint.position);
        GameObject track = Instantiate(originTrack, trackPosition, Quaternion.identity);
        track.transform.localScale = track.transform.localScale * 2.5f; // 스케일 조정


        generatedTracks.Add(track.GetComponentInChildren<TrackElement>());
    }

    [ContextMenu("AutoSetupTracks")]
    private void AutoSetupTracks()
    {
        string[] searchFolders = { "Assets/Resources/prefab/Track" };
        string[] guids = AssetDatabase.FindAssets("t:Prefab", searchFolders);
        List<GameObject> prefabList = new List<GameObject>(); 
        foreach (string guid in guids)
        {
            // 3. GUID를 실제 에셋 경로(Path)로 변환
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // 4. 경로에 있는 프리팹을 GameObject 형태로 로드
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                if(prefab.name.Contains("Track_") && !prefab.name.Contains("old"))
                {
                    prefabList.Add(prefab);
                    Debug.Log($"프리팹 로드 완료: {prefab.name} ({assetPath})");
                }
            }
        }

        trackPrefabs = prefabList.ToArray();
    }
}
