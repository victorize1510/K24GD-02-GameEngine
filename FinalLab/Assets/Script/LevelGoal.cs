using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[DefaultExecutionOrder(-90)]
public class LevelGoal : MonoBehaviour
{
    public static LevelGoal Instance { get; private set; }

    [Header("Next Level")]
    [Tooltip("Để trống = load scene kế tiếp trong Build Settings")]
    public string nextSceneName = "";

    [Header("Debug")]
    [SerializeField] int aliveEnemies;    
    public bool AllCleared => aliveEnemies <= 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        aliveEnemies = 0;
    }

    // ---- API cho EnemyRegister gọi ----
    public void RegisterEnemy()
    {
        aliveEnemies++;
    }
    public void UnregisterEnemy()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }

    // Gọi khi Player chạm VictoryPoint
    public void TryWin()
    {
        if (!AllCleared)
        {
            Debug.Log("[LevelGoal] Chưa thắng: vẫn còn quái sống.");
            return;
        }

        // Nếu có VictoryUI trong scene, cho UI hiện; nếu không thì load luôn
        var ui = FindObjectOfType<VictoryUI>(true);
        if (ui) ui.Show();
        else LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        if (ScoreManager.Instance) ScoreManager.Instance.ResetScore();

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}