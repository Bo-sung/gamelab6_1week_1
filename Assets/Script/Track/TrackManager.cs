using Unity.VisualScripting;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public GameObject[] trackPrefabs;

    public TrackElement[] generatedTracks;

    private TrackEdge[] trackEdges;

    public System.Action OnCarTrackFinish;

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

    private Vector2[] EdgePositionOffset =
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
        GameObject track;

        if(currentTrack % 2 == 0)
        {
            int randomInt = Random.Range(0, trackPrefabs.Length);
            if(trackPrefabs[randomInt].GetComponent<TrackElement>().IsCurve == true)
            {
                track = Instantiate(trackPrefabs[randomInt]);
                track.transform.rotation = Quaternion.Euler(0, 90 * currentTrack / 2, 0);
                track.transform.position = trackPositionOffset[currentTrack];
            }
        }
        else
        {
            int randomInt = Random.Range(0, trackPrefabs.Length);
            if (trackPrefabs[randomInt].GetComponent<TrackElement>().IsCurve == true)
            {
                track = Instantiate(trackPrefabs[randomInt]);
                track.transform.rotation = Quaternion.Euler(0, 90 * currentTrack / 2 - 1, 0);
                track.transform.position = trackPositionOffset[currentTrack];
            }
        }

        currentTrack++;
        if (currentTrack == 8)
            currentTrack = 0;
    }
}
