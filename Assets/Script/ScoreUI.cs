using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI score;
    [SerializeField]
    private int scoreValue = 0;


    private void Awake()
    {

    }
    private void Update()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        score.text = "Score: " + scoreValue;
    }

    public void ApplyScore(int score)
    {
        scoreValue += score;
    }
}
