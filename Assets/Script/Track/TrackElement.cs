using UnityEngine;
using UnityEngine.UI;

public class TrackElement : MonoBehaviour
{
    [SerializeField]
    private TrackEdge trackStart;
    [SerializeField]
    private TrackEdge trackEnd;
    [SerializeField]
    private float life = 10000f;
    private bool IsCarTrackOut = false;

    public System.Action OnCarTrackIn;
    public System.Action OnCarTrackOut;

    private void Awake()
    {
        trackStart.OnTrigger += () => OnCarTrackIn?.Invoke();
        trackEnd.OnTrigger += () => OnCarTrackOut?.Invoke();
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }

    private void Update()
    {
        // 트랙 아웃 상태가 아닌 경우에는 생명 주기 감소를 하지 않음
        if (!IsCarTrackOut)
            return;
        // 트랙 아웃시 사망 카운터 시작
        life -= Time.deltaTime;
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }
}