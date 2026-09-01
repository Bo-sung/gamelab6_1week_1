using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class TrackManager : MonoBehaviour
{
    public GameObject[] trackPrefabs;

    public List<TrackElement> generatedTracks;

    public TrackEdge trackEdge;

    private int currentTrackCount = 0;

    public System.Action OnCarTrackFinish;

    private void Awake()
    {
        for(int i=0; i<3; i++)
        {
            Generate();
        }

    }

    public void Initialize()
    {
        // 초기 트랙 생성

    }

    public void Generate()
    {
        // OnCarTrackFinish > GameManager의 Evenet > Generate()
        // 트랙 생성

        // trackElement의 StartPoint에 Edge가 이동
        trackEdge.transform.position = generatedTracks[currentTrackCount].endPoint.position;

        // trackPrefab의 중심이 startPoint라는 가정 하에
        GameObject originTrack = trackPrefabs[Random.Range(0, trackPrefabs.Length - 1)];
        Vector3 trackPosition= transform.TransformPoint(generatedTracks[generatedTracks.Count - 1].endPoint.position);
        Quaternion trackRotation = generatedTracks[generatedTracks.Count - 1].endPoint.rotation;
        GameObject track = Instantiate(originTrack, trackPosition, trackRotation);


        generatedTracks.Add(track.GetComponentInChildren<TrackElement>());
    }
}
