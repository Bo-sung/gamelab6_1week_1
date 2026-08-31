using Unity.VisualScripting;
using UnityEngine;

public class TrackGenerate : MonoBehaviour
{
    public GameObject[] trackPrefabs;

    public TrackElement[] generatedTracks;

    private TrackEdge[] trackEdges;

    private int currentTrack = 0;

    private Vector2[] trackPositionOffset =
    {
        new Vector2Int(0, 0),
        new Vector2Int(0, 10),
        new Vector2Int(0, 20),
        new Vector2Int(10, 20),
        new Vector2Int(20, 20),
        new Vector2Int(20, 10),
        new Vector2Int(20, 0),
        new Vector2Int(10, 0)
    };
    
    private void Start()
    {
        // 시작할때는 고정적으로 배치하는 편이 나을듯(시작부터 괴랄한 트랙 나오면 다소 이상할 수 있음)
        for(int i = 0; i<3; i++)
        {
            Generate();
        }
    }

    public void Generate()
    {
        GameObject track = Instantiate(trackPrefabs[Random.Range(0, 1)]); // 랜덤값 나중에 변경

        // position 설정
        track.transform.position = trackPositionOffset[currentTrack];
        // rotation 설정
        if (currentTrack % 2 == 0)
        {
            track.transform.rotation = Quaternion.Euler(0, currentTrack/2, 0);
        }

        currentTrack++;
        if (currentTrack == 8)
            currentTrack = 0;
    }
}
