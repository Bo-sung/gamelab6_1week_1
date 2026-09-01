using TMPro;
using UnityEngine;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private GameObject GameOverPanel;
    [SerializeField]
    private GameObject GameOverText;
    [SerializeField]
    private TextMeshProUGUI ScoreText;

    [ContextMenu("게임 오버 UI 띄우기")]
    public void StartGameOverUI(int score)
    {
        StartCoroutine(ShowGameOver(score));
    }

    private IEnumerator ShowGameOver(int score)
    {
        GameOverPanel.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        GameOverText.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        ScoreText.text = "Score: " + score.ToString();
    }
}
