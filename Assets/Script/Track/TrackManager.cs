using Unity.VisualScripting;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    public GameObject[] trackPrefabs;

    public TrackElement[] generatedTracks;

    public TrackEdge[] trackEdges;

    public System.Action OnCarTrackFinish;

    private int currentTrack = 0;

    private int currentDestroy = 0;

    private Vector3[] EdgePositionOffset =
    {
        new Vector3(5, 0, 0),
        new Vector3(15, 0, 0),
        new Vector3(20, 0, 5),
        new Vector3(20, 0, 15),
        new Vector3(15, 0, 20),
        new Vector3(5, 0, 20),
        new Vector3(0, 0, 15),
        new Vector3(0, 0, 5)
    };

    private Vector3[] trackPositionOffset =
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 10),
        new Vector3(0, 0, 20),
        new Vector3(10, 0, 20),
        new Vector3(20, 0, 20),
        new Vector3(20, 0, 10),
        new Vector3(20, 0, 0),
        new Vector3(10, 0, 0)
    };

    private void Awake()
    {
        OnCarTrackFinish += Generate;
    }

    private void Start()
    {
        // 시작할때는 고정적으로 배치하는 편이 나을듯(시작부터 괴랄한 트랙 나오면 다소 이상할 수 있음)
        for(int i = 0; i<3; i++)
        {
            Generate();
        }
        currentDestroy = 0;

        for (int i = 0; i < 8; i++) 
        {
            trackEdges[i].transform.position = EdgePositionOffset[i];
            trackEdges[i].OnTrigger += () => OnCarTrackFinish?.Invoke();
        }
    }

    public void Generate()
    {
        currentDestroy++;
        if (currentDestroy == 8)
            currentDestroy = 0;

        GameObject track;

        Debug.Log("!");
        while(true)
        {
            if (currentTrack % 2 == 0)
            {
                int randomInt = Random.Range(0, trackPrefabs.Length);
                if (trackPrefabs[randomInt].GetComponent<TrackElement>().isCurve == true)
                {
                    track = Instantiate(trackPrefabs[randomInt]);
                    track.transform.rotation = Quaternion.Euler(0, 90 * currentTrack / 2, 0);
                    track.transform.position = trackPositionOffset[currentTrack];
                    generatedTracks[currentTrack] = track.GetComponent<TrackElement>();
                    generatedTracks[currentDestroy]?.DestroyCountdown();
                    break;
                }
            }
            else
            {
                int randomInt = Random.Range(0, trackPrefabs.Length);
                if (trackPrefabs[randomInt].GetComponent<TrackElement>().isCurve == false)
                {
                    track = Instantiate(trackPrefabs[randomInt]);
                    track.transform.rotation = Quaternion.Euler(0, 90 * (currentTrack - 1) / 2, 0);
                    track.transform.position = trackPositionOffset[currentTrack];
                    generatedTracks[currentTrack] = track.GetComponent<TrackElement>();
                    generatedTracks[currentDestroy]?.DestroyCountdown();
                    break;
                }
            }
        }

        currentTrack++;
        if (currentTrack == 8)
            currentTrack = 0;


    }
}
