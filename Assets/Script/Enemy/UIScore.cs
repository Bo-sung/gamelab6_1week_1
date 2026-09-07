using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameScene;

public class UIScore : MonoBehaviour, IScoreUI
{
    [SerializeField]
    TextMeshProUGUI txt_Score;
    [SerializeField]
    TextMeshProUGUI txt_Combo;
    [SerializeField]
    TextMeshProUGUI txt_MaxCombo;
    [SerializeField]
    Image gauge;
    IScoreUI.IPresenter presenter;

    public void Initialize(IScoreUI.IPresenter presenter)
    {
        this.presenter = presenter;
        Refresh();
    }

    public void Refresh()
    {
        txt_Score.text = presenter.Score().ToString();
        txt_Combo.text = presenter.Combo().ToString();
        txt_MaxCombo.text = presenter.MaxCombo().ToString();
        gauge.fillAmount = 0;
    }

    void Update()
    {
        gauge.fillAmount = presenter.GaugeVal();
    }
}

