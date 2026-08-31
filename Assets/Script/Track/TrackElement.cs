using UnityEngine;
using UnityEngine.UI;

public class TrackElement : MonoBehaviour
{
    [SerializeField]
    private TrackEdge trackStart;
    [SerializeField]
    private TrackEdge trackEnd;


    public System.Action OnCarTrackIn;
    public System.Action OnCarTrackOut;


    private void Awake()
    {
        trackStart.OnTrigger += () => OnCarTrackIn?.Invoke();
        trackEnd.OnTrigger += () => OnCarTrackOut?.Invoke();
    }
}
