using UnityEngine;
using static GameScene;

public class UIHealth_Presenter : IHealthUI.Presenterbase
{
    public UIHealth_Presenter(IStatManager manager) : base(manager)
    {

    }

    public override int GetHealth()
    {
        return statManager.GetHP();
    }

    public override int GetMaxHealth()
    {
        return statManager.GetMaxHP();
    }
}

public class GameScene : MonoBehaviour, IStatManager
{
    public interface IUI
    {
        void Refresh();
    }

    public interface IScoreUI: IUI
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
    public interface IHealthUI : IUI
    {
        public abstract class Presenterbase
        {
            public abstract int GetHealth();
            public abstract int GetMaxHealth();

            protected IStatManager statManager;
            public System.Action OnRefreshRequired;
            public Presenterbase(IStatManager statManager)
            {
                this.statManager = statManager;
            }
        }

        void Initialize(Presenterbase presenter);
    }
    public interface IWaveUI : IUI
    {
        void SetData(int maxWave);
        void UpdateData(WaveInfo info);
    }
    public interface IGameResultUI : IUI
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

    public interface IMiniMap : IUI
    {
        void AddElement(IMapElement element);
    }

    [SerializeField]
    Player player;

    [SerializeField]
    private int curPlayerHealth = 10;
    [SerializeField]
    private int maxPlayerHealth = 10;

    // UI 객체
    IScoreUI scoreUI;
    IHealthUI healthUI;
    IWaveUI waveUI;
    IGameResultUI gameResultUI;
    IMiniMap miniMap;
    private bool gameEnded;

    // 매니저 객체
    WaveManager m_waveManager;
    ScoreManager m_scoreManager;

    // 외부 노출용
    public WaveManager waveManager => m_waveManager;
    public ScoreManager scoreManager => m_scoreManager;

    // 이벤트
    public System.Action<WaveInfo> OnWaveClear;
    public System.Action OnWaveStart;
    public System.Action OnHPChanged;

    void Awake()
    {
        // 매니저 초기화
        m_scoreManager = GetComponent<ScoreManager>();
        m_waveManager = GetComponent<WaveManager>();

        // ui 초기화
        waveUI = GetComponentInChildren<IWaveUI>(true);
        scoreUI = GetComponentInChildren<IScoreUI>(true);
        healthUI = GetComponentInChildren<IHealthUI>(true);
        gameResultUI = GetComponentInChildren<IGameResultUI>(true);
        miniMap = GetComponentInChildren<IMiniMap>(true);

        scoreUI.Initialize(new UIScore_Presenter(m_scoreManager));
        healthUI.Initialize(new UIHealth_Presenter(this));

        // 커서 정렬
        Cursor.lockState = CursorLockMode.None; // 화면 중앙에 고정
        Cursor.visible = true;

        // 스텟 초기화
        curPlayerHealth = maxPlayerHealth;

        player.Initialize(this);

        AddEvents();
        UIStart();
    }

    void Start()
    {
        waveManager.Initialize(player.transform, scoreManager, miniMap);
        scoreManager.SetScore(0);
        waveManager.StartGame();
    }

    private void UIStart()
    {
        waveUI.SetData(m_waveManager.WaveInfos.Length);
        healthUI.Refresh();
        scoreUI.Refresh();
        waveUI.Refresh();
        gameResultUI?.Refresh();
        miniMap.Refresh();
    }

    private void AddEvents()
    {
        m_waveManager.OnWaveClear += Handle_WaveClear;
        m_waveManager.OnWaveClear += waveUI.UpdateData;

        m_waveManager.OnWaveStart += Handle_WaveStart;
        m_waveManager.OnWaveEmpty += HandleGameClear;
        m_scoreManager.OnScoreChanged += HandleOnScore;
    }

    private void HandleOnScore()
    {
        waveUI.Refresh();
    }

    private void Handle_WaveStart()
    {
        OnWaveStart?.Invoke();
    }

    private void Handle_WaveClear(WaveInfo info)
    {
        if (gameEnded)
            return;
        ApplyHP(1);
        waveManager.NextWave(info);
        OnWaveClear?.Invoke(info);
    }

    public int GetHP()
    {
        return curPlayerHealth;
    }

    public void SetHP(int _hp)
    {
        if (gameEnded)
            return;
        curPlayerHealth = Mathf.Clamp(_hp, 0, maxPlayerHealth);
        healthUI?.Refresh();
        OnHPChanged?.Invoke();
        if (curPlayerHealth == 0)
            EndGame(false);
    }

    private void HandleGameClear() => EndGame(true);

    private void EndGame(bool success)
    {
        if (gameEnded)
            return;
        gameEnded = true;
        waveManager.StopAllCoroutines();
        player.StopGameplay();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameResultUI?.UpdateData(scoreManager.GetScore(), success ? "GAME CLEAR" : "GAME OVER");
        gameResultUI?.Refresh();
    }

    public void ApplyHP(int _hp)
    {
        SetHP(curPlayerHealth + _hp);
    }

    public int GetMaxHP()
    {
        return maxPlayerHealth;
    }
}
