using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverWatcher : MonoBehaviour
{
     public PlayerController player;
    public GameOverUI gameOverUI;

    bool shown;

    void OnEnable()
    {
        // reset mỗi lần enable
        shown = false;

        // (re)bind refs
        if (!player) player = FindObjectOfType<PlayerController>();
        if (!gameOverUI) gameOverUI = FindObjectOfType<GameOverUI>(true);

        // lắng nghe khi load scene (restart)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        // mỗi lần scene mới vào → reset + rebind
        shown = false;
        player = FindObjectOfType<PlayerController>();
        gameOverUI = FindObjectOfType<GameOverUI>(true);

        // bảo đảm không còn pause từ scene trước
        if (Mathf.Approximately(Time.timeScale, 0f))
            Time.timeScale = 1f;
    }

    void Update()
    {
        if (shown || !player || !gameOverUI) return;

        // điều kiện die: CurrentHP <= 0 hoặc player.dead (tuỳ bạn)
        if (player.CurrentHP <= 0)
        {
            shown = true;
            gameOverUI.Show();   // Show() sẽ tự Time.timeScale = 0f
        }
    }
}