using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    public GameObject gameOverPanel;
    public GameObject winPanel;

    public TMP_Text gameOverScoreText;
    public TMP_Text gameOverHighScoreText;

    public TMP_Text winScoreText;
    public TMP_Text winHighScoreText;

    void Awake()
    {
        gameOverPanel.SetActive(false);
        winPanel.SetActive(false);
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void GameOver()
    {
        
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        // Hiện điểm
        gameOverScoreText.text = "Score: " + ScoreManager.instance.score;
        gameOverHighScoreText.text = "HighScore: " + ScoreManager.instance.highScore;
    }

    public void WinGame()
    {
        ;
        winPanel.SetActive(true);
        Time.timeScale = 0f;
        // Hiện điểm
        winScoreText.text = "Score: " + ScoreManager.instance.score;
        winHighScoreText.text = "HighScore: " + ScoreManager.instance.highScore;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        ScoreManager.instance.ResetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
