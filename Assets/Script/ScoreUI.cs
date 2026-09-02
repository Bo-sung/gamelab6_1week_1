using TMPro;
using UnityEngine;
using UnityEditor;
using System.Text;
using System;

public class ScoreUI : MonoBehaviour, IScoreHandler
{
    private const string COMBO_STR = "Combo : ";
    private const string MAX_COMBO_STR = "Max Combo : ";
    private const string SCORE_STR = "Score : ";
    [SerializeField]
    private TextMeshProUGUI txt_Score;
    [SerializeField]
    private int scoreValue = 0;

    [Header("Combo")]
    [SerializeField]
    private TextMeshProUGUI txt_Combo;
    [SerializeField]
    private TextMeshProUGUI txt_MaxCombo;
    [SerializeField]
    private float comboRemainTimeMax = 3f;
    [SerializeField]
    private float curComboRemainTime = 0f;
    [SerializeField]
    private int curCombo = 0;

    [SerializeField]
    private int maxCombo = 0;


    private void Awake()
    {

    }
    private void Update()
    {
        UpdateScore();
        UpdateCombo();
    }

    public void UpdateScore()
    {
        //UI 갱신
        txt_Score.text = SCORE_STR + scoreValue;
        txt_Combo.text = COMBO_STR + curCombo;
        txt_MaxCombo.text = MAX_COMBO_STR + maxCombo;
    }

    private void UpdateCombo()
    {
        curComboRemainTime -= Time.deltaTime;
    }

    public int GetCombo()
    {
        return curCombo;
    }

    public void ApplyScore(int score)
    {
        scoreValue += score;
        // 콤보 남은 시간이 0 미만이면 콤보 저장 후 새로 세팅
        if (curComboRemainTime <= 0)
        {
            // 콤보 초기화
            curCombo = 0;
        }
        // 시간 재갱신
        curComboRemainTime = comboRemainTimeMax;
        curCombo++;
        // 최대 콤보보다 현재가 더 많으면 갱신
        maxCombo = maxCombo < curCombo ? curCombo : maxCombo;

    }
}
