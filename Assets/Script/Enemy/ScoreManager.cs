using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour, IScoreHandler
{
    [SerializeField]
    private int score = 0;
    public int Score => score;

    private int combo = 0;
    public int Combo => combo;
    private int maxCombo = 0;
    public int MaxCombo => maxCombo;

    [SerializeField]
    private double comboDuration;
    private DateTime lastScoreTime = DateTime.Now;

    public float GaugeVal => (float)(1 - (lastScoreTime - DateTime.Now).Milliseconds / comboDuration);

    public void ApplyScore(int score)
    {
        this.score += score;
        var passedTime = lastScoreTime - DateTime.Now;

        comboDuration = passedTime.TotalMilliseconds;

        combo = passedTime.TotalMilliseconds > comboDuration ? 1 : combo + 1;

        this.maxCombo = Math.Max(combo, maxCombo);
        this.lastScoreTime = DateTime.Now;
    }

    public int GetScore()
    {
        return score;
    }

    public void SetScore(int score)
    {
        this.score = score;
    }
}

