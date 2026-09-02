using UnityEditor.Build;
using UnityEngine;

public class ComboManager : MonoBehaviour, IScoreHandler
{
    [Header("combo Values")]
    [SerializeField]
    private int maxCombo = 0;
    [SerializeField]
    private float comboRemainTimeMax = 3f;
  

    private ArrowController arrow;

    private int curScore;
    private int currentCombo;
    private float comboRemainTime;

    public void Initialize(ArrowController con)
    {
        arrow = con;
    }

    public void Update()
    {
        comboRemainTime -= Time.deltaTime;
        if (comboRemainTime <= 0f)
        {
            currentCombo = 0;
            comboRemainTime = 0;
        }

    }


    public void ApplyScore(int score)
    {
        curScore += score;
        currentCombo++;
        comboRemainTime = comboRemainTimeMax;
        
    }

    public int GetCombo()
    {
        return currentCombo;
    }

    public int GetScore()
    {
        return curScore;
    }


 
}
