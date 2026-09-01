using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI score;
    static public int scoreValue = 0;


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
}
