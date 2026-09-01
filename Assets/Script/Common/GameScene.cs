using UnityEngine;

public class GameScene : MonoBehaviour
{
    [SerializeField]
    private float startDelay = 5f;
    private float waitTime;
    private bool isChasing;

    [SerializeField]
    GameOverWallController GameoverWall;

    [SerializeField]
    private CarController carController;

    private TrackManager trackManager;

    public System.Action OnGameOverWallReachedPlayer;

    private bool isReady = false;
    private void Update()
    {
        if(!isReady)
            return;
        if (isChasing)
            return;

        waitTime += Time.deltaTime;
        if (waitTime >= startDelay)
            GameoverWall.gameObject.SetActive(true);    // 게임 시작 후 일정 시간 후에 GameOverWall 활성화
    }
    public void Ready(TrackManager trackManager)
    {
        this.trackManager = trackManager;
        trackManager.Initialize();
        GameoverWall.transform.position = new Vector3(0, 0, 0); // 초기 위치 설정
        carController.transform.position = new Vector3(0, 0, 0); // 초기 위치 설정

        GameoverWall.OnGameOverWallReachedPlayer += () => OnGameOverWallReachedPlayer?.Invoke();
        trackManager.OnCarTrackFinish += trackManager.Generate;
    }

    public void OnStart()
    {
        isReady = true;
    }
}
