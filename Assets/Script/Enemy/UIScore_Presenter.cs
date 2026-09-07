using static GameScene;

public class UIScore_Presenter : IScoreUI.IPresenter
{
    ScoreManager scoreManager;

    public UIScore_Presenter(ScoreManager manager)
    {
        this.scoreManager = manager;
    }

    public int Score() => scoreManager.Score;
    public int Combo() => scoreManager.Combo;
    public int MaxCombo() => scoreManager.MaxCombo;
    public float GaugeVal() => scoreManager.GaugeVal;
}

