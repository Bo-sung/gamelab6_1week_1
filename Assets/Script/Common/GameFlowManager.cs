using UnityEngine;
using UnityEngine.SceneManagement;
// 게임 시작부터 끝까지의 흐름을 관리하는 클래스
[DisallowMultipleComponent]
public class GameFlowManager : MonoBehaviour
{
    private const string GAME_SCENE_NAME = "GameScene";
    private const string PRELOAD_SCENE_NAME = "preload";
    private const string TITLE_SCENE_NAME = "Title";

    [SerializeField]
    private TitleUI titleUI;

    AugmentManger agManager;
    CarStat carStat;
    TrackManager trackManager;

    private Scene currentScene;

    // 프로세스 시작점
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        currentScene = SceneManager.GetActiveScene();
    }

    private void Start()
    {
        if (currentScene.name == PRELOAD_SCENE_NAME)
        {
            SceneManager.LoadScene(TITLE_SCENE_NAME);
        }
    }

    //  씬이 로드될 때 호출되는 이벤트 핸들러
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == TITLE_SCENE_NAME)
        {
            titleUI = FindAnyObjectByType<TitleUI>();
            titleUI.AddEvent();
            titleUI.OnGameStartBtnClicked += GameSceneLoadStart;
        }
        else if (arg0.name == GAME_SCENE_NAME)
        {
            // 씬이 로드될 때 필요한 컴포넌트들을 찾아서 초기화
            if (agManager == null)
                agManager = FindAnyObjectByType<AugmentManger>();
            if (trackManager == null)
                trackManager = FindAnyObjectByType<TrackManager>();
            if (carStat == null)
                carStat = FindAnyObjectByType<CarStat>();
            // 이벤트 핸들러를 등록하기 전에 기존 이벤트 핸들러를 제거하여 중복 호출 방지
            agManager.OnSelectAugment = null;
            agManager.OnSelectAugment += HandleAugmentSelected;
            trackManager.OnCarTrackFinish = null;
            trackManager.OnCarTrackFinish += HandleCarTrackFinish;

            // GameScene의 Ready 메서드를 호출하여 트랙 매니저를 전달
            var gameScene = FindAnyObjectByType<GameScene>();
            gameScene.Ready(trackManager);
        }
        // 씬이 전환될 때 현재 씬을 업데이트
        currentScene = SceneManager.GetActiveScene();
    }

    // Augment 선택 시 호출되는 이벤트 핸들러
    private void HandleAugmentSelected(Augment data)
    {
        switch (data.type)
        {
            case AugmentType.SpeedUp:
                carStat.ApplySpeed(data.value);
                Debug.Log("SpeedUp " + data.value);
                break;
            case AugmentType.AccelerationUp:
                carStat.ApplyAcceleration(data.value);
                Debug.Log("AccelerationUp " + data.value);
                break;
            case AugmentType.BrakeUp:
                carStat.ApplyBraking(data.value);
                Debug.Log("BrakeUp " + data.value);
                break;
            case AugmentType.CorneringUp:
                carStat.ApplyCornering(data.value);
                Debug.Log("CorneringUp " + data.value);
                break;
            case AugmentType.WeightUp:
                carStat.ApplyWeight(data.value);
                Debug.Log("WeightUp " + data.value);
                break;
            case AugmentType.LoseWheel:
                Debug.Log("LoseWheel");
                break;
            case AugmentType.ThrowBreak:
                Debug.Log("ThrowBreak");
                break;
            case AugmentType.SpringBumper:
                Debug.Log("SpringBumper");
                break;
        }
    }

    // Car가 트랙을 완주했을 때 호출되는 이벤트 핸들러
    private void HandleCarTrackFinish()
    {
        agManager.ActiveScreen();
    }
    private void GameSceneLoadStart()
    {
        SceneManager.LoadScene(GAME_SCENE_NAME);
        Debug.Log("GameSceneLoadStart");
    }
}

// 플레이어의 현재 상태 나타내는 클래스
public class PlayerStatus
{
    private int hp;
    private int trackProgress;
    private int augmentCount;

    public void SetHP(int hp) { this.hp = hp; }
    public int HP => hp;

    public void SetTrackProgress(int progress) { this.trackProgress = progress; }
    public int TrackProgress => trackProgress;

    public void SetAugmentCount(int count) { this.augmentCount = count; }
    public int AugmentCount => augmentCount;
}

public class GameProgress
{
    public int TrackProgress { get; set; } = 0;
    public int AugmentCount { get; set; } = 0;
    public Vector3 GameOverWallPosition { get; set; } = Vector3.zero;
}
