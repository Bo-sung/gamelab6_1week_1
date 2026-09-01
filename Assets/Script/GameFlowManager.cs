using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public Player player;
    public GameOverUI gameOverUI;

    private void Awake()
    {
        player.OnPlayerDead += GameOver;
    }

    private void GameOver()
    {
        player.gameObject.SetActive(false);
        Debug.Log("Game Over");
    }
}
