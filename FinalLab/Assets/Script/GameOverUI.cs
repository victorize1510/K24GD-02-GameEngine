using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class GameOverUI : MonoBehaviour
{
    public TMP_Text scoreText;
    public Button restartButton;

    void Awake()
    {
        gameObject.SetActive(false);
        if (restartButton) restartButton.onClick.AddListener(Restart);
    }

    public void Show()
    {
        if (scoreText) scoreText.text = $"Score: {ScoreManager.Instance?.Score ?? 0}";
        gameObject.SetActive(true);
        Time.timeScale = 0f;   // pause sau khi panel hiện
    }

    void Restart()
    {
        Time.timeScale = 1f;   // bỏ pause trước khi load
        ScoreManager.Instance?.ResetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}