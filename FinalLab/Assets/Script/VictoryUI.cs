using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text scoreText;
    public Button primaryButton;
    public TMP_Text primaryButtonLabel;

    [Header("Mode")]
    public bool finalLevel = false;             // Level 2: bật true
    public string restartSceneName = "Level 1";  // Tên scene để Restart về

    [Header("Fallback (non-final levels)")]
    public string nextSceneName;

    private LevelGoal goal;

    void Awake()
    {
        if (!primaryButton) primaryButton = GetComponentInChildren<Button>(true);
        if (primaryButton)
        {
            primaryButton.onClick.RemoveAllListeners();
            primaryButton.onClick.AddListener(OnPrimaryClick);
        }
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (!primaryButtonLabel && primaryButton)
            primaryButtonLabel = primaryButton.GetComponentInChildren<TMP_Text>(true);
        if (primaryButtonLabel)
            primaryButtonLabel.text = finalLevel ? "Restart" : "Next";

        if (!goal) goal = FindObjectOfType<LevelGoal>(true);
    }

    void OnDisable()
    {
        if (Mathf.Approximately(Time.timeScale, 0f))
            Time.timeScale = 1f;
    }

    public void Show()
    {
        if (!scoreText) scoreText = GetComponentInChildren<TMP_Text>(true);
        if (scoreText)
        {
            int score = ScoreManager.Instance ? ScoreManager.Instance.Score : 0;
            scoreText.text = $"Score: {score}";
        }

        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    void OnPrimaryClick()
    {
        Time.timeScale = 1f;

        if (finalLevel)
        {
            // 1) Reset điểm cho run mới
            if (ScoreManager.Instance) ScoreManager.Instance.ResetScore();

            // 2) Heal player sau khi scene restart được load (kể cả player persist hay spawn mới)
            SceneManager.sceneLoaded += HealPlayerAfterLoad;

            // 3) Load về Level 1
            if (!string.IsNullOrEmpty(restartSceneName))
                SceneManager.LoadScene(restartSceneName);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // fallback
            return;
        }

        // Chưa-final: sang màn kế
        if (!goal) goal = FindObjectOfType<LevelGoal>(true);
        if (goal) { goal.LoadNextLevel(); return; }

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void HealPlayerAfterLoad(Scene s, LoadSceneMode m)
    {
        SceneManager.sceneLoaded -= HealPlayerAfterLoad;

        var player = FindObjectOfType<PlayerController>(true);
        if (player) player.FullHeal();   // luôn full máu khi bắt đầu lại Level 1
    }
}