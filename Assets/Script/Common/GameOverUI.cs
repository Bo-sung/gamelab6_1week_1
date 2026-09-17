using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour, GameScene.IGameResultUI
{
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private GameObject GameOverText;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private Button rtBtn;
    [SerializeField] private Button totitleBtn;
    [SerializeField] private string titleSceneName = "Title";

    private int resultScore;
    private string resultText;
    private bool hasResult;
    private bool loadingScene;
    private Coroutine showCoroutine;

    private void Awake()
    {
        rtBtn.onClick.AddListener(RestartGame);
        totitleBtn.onClick.AddListener(GoToTitle);
        if (!hasResult)
            HideElements();
    }

    private void OnDestroy()
    {
        rtBtn.onClick.RemoveListener(RestartGame);
        totitleBtn.onClick.RemoveListener(GoToTitle);
    }

    public void StartGameOverUI(int score) => ShowResult(score, false);
    public void StartGameClearUI(int score) => ShowResult(score, true);

    public void ShowResult(int score, bool success)
    {
        UpdateData(score, success ? "GAME CLEAR" : "GAME OVER");
        Refresh();
    }

    public void UpdateData(int score, string text)
    {
        resultScore = score;
        resultText = text;
        hasResult = true;
    }

    public void Refresh()
    {
        if (!hasResult)
            return;

        gameObject.SetActive(true);
        if (showCoroutine != null)
            StopCoroutine(showCoroutine);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        showCoroutine = StartCoroutine(ShowGameOver());
    }

    private void HideElements()
    {
        GameOverPanel.SetActive(false);
        GameOverText.SetActive(false);
        ScoreText.gameObject.SetActive(false);
        rtBtn.gameObject.SetActive(false);
        totitleBtn.gameObject.SetActive(false);
    }

    private IEnumerator ShowGameOver()
    {
        HideElements();
        GameOverPanel.SetActive(true);
        GameOverText.GetComponentInChildren<TMP_Text>(true).text = resultText;
        ScoreText.text = "Score: " + resultScore;
        yield return new WaitForSecondsRealtime(0.3f);
        GameOverText.SetActive(true);
        yield return new WaitForSecondsRealtime(0.3f);
        ScoreText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.3f);
        rtBtn.gameObject.SetActive(true);
        totitleBtn.gameObject.SetActive(true);
        showCoroutine = null;
    }

    public void RestartGame() => LoadScene(SceneManager.GetActiveScene().name);
    public void GoToTitle() => LoadScene(titleSceneName);

    private void LoadScene(string sceneName)
    {
        if (loadingScene)
            return;
        loadingScene = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(sceneName);
    }
}