using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameScene : MonoBehaviour
{
    public interface IScoreUI
    {
        public interface IPresenter
        {
            int Score();
            int Combo();
            int MaxCombo();
            float GaugeVal();
        }
        void Initialize(IPresenter presenter);
    }
    public interface IHealthUI
    {
        void SetData(int maxHealth);

        void ApplyHealth(int health);

        void SetHealth(int health);
    }
    public interface IWaveUI
    {
        void SetData(int maxWave);
        void UpdateData(WaveInfo info);
    }
    public interface IGameResultUI
    {
        void UpdateData(int score, string text);
    }

    public interface IMapElement
    {
        Transform GetTransform();

        Color GetMarkercolor();

        float GetMarkerSize();

        bool IsCenterTarget();
    }

    public interface IMiniMap
    {
        void AddElement(IMapElement element);
    }

    [SerializeField]
    Player player;

    // UI 객체
    IScoreUI scoreUI;
    IHealthUI healthUI;
    IWaveUI waveUI;
    IGameResultUI gameResultUI;
    IMiniMap miniMap;

    // 매니저 객체
    WaveManager m_waveManager;
    ScoreManager m_scoreManager;

    // 외부 노출용
    public WaveManager waveManager => m_waveManager;
    public ScoreManager scoreManager => m_scoreManager;

    // 이벤트
    public System.Action<WaveInfo> OnWaveClear;
    public System.Action OnWaveStart;

    void Awake()
    {
        // 매니저 초기화
        m_waveManager = GetComponent<WaveManager>();
        m_scoreManager = GetComponent<ScoreManager>();

        // ui 초기화
        waveUI = GetComponentInChildren<IWaveUI>(true);
        scoreUI = GetComponentInChildren<IScoreUI>(true);
        healthUI = GetComponentInChildren<IHealthUI>(true);
        gameResultUI = GetComponentInChildren<IGameResultUI>(true);
        miniMap = GetComponentInChildren<IMiniMap>(true);

        scoreUI.Initialize(new UIScore_Presenter(m_scoreManager));

        // 커서 정렬
        Cursor.lockState = CursorLockMode.None; // 화면 중앙에 고정
        Cursor.visible = true;

        AddEvents();
        UIStart();
    }

    void Start()
    {
        waveManager.Initialize(player.transform, scoreManager, miniMap);
        waveManager.StartGame();
    }

    private void UIStart()
    {
        waveUI.SetData(m_waveManager.WaveInfos.Length);
    }

    private void AddEvents()
    {
        m_waveManager.OnWaveClear += Handle_WaveClear;
        m_waveManager.OnWaveClear += waveUI.UpdateData;

        m_waveManager.OnWaveStart += Handle_WaveStart;
    }

    private void Handle_WaveStart()
    {
        OnWaveStart?.Invoke();
    }

    private void Handle_WaveClear(WaveInfo info)
    {
        player.Heal();
        waveManager.NextWave(info);
        OnWaveClear?.Invoke(info);
    }
}
