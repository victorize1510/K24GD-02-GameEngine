using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public int score;
    public int highScore;
    public static ScoreManager instance;

    void Start()
    {
        instance = this;
        // Load HighScore từ PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GetScore(int value)
    {
        score += value;

        // Cập nhật HighScore nếu vượt qua
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        UpdateUI();
    }
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();

        if (highScoreText != null)
            highScoreText.text = "HighScore: " + highScore.ToString();
    }
     public void ResetScore()
    {
        score = 0;
        scoreText.text = "Score: " + score.ToString();
    }

    public int GetCurrentScore()
    {
        return score;
    }

    public int GetHighScore()
    {
        return highScore;
    }
}
