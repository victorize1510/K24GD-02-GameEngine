using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-100)]          // chạy sớm để kịp nhận đăng-ký
public class Level2Goal : MonoBehaviour
{
    public static Level2Goal Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Kéo VictoryUI (panel) vào. Để trống sẽ tự FindObjectOfType(true).")]
    public VictoryUI victoryUI;
    public bool autoFindUI = true;

    [Header("Events")]
    public UnityEvent OnVictory;       // tuỳ chọn: sound/FX bổ sung

    [Header("Debug (read-only)")]
    [SerializeField] int  aliveEnemies = 0;
    [SerializeField] bool bossPresent  = false;
    [SerializeField] bool bossDead     = false;
    [SerializeField] bool victoryShown = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        aliveEnemies = 0; bossPresent = false; bossDead = false; victoryShown = false;

        // nếu đang pause từ scene trước thì bỏ pause
        if (Mathf.Approximately(Time.timeScale, 0f)) Time.timeScale = 1f;

        if (!victoryUI && autoFindUI) victoryUI = FindObjectOfType<VictoryUI>(true);
    }

    // -------- API cho Enemy/Boss gọi --------
    public void RegisterEnemy(bool isBoss)
    {
        aliveEnemies++;
        if (isBoss) bossPresent = true;
        // Debug.Log($"[Level2Goal] +1 (boss? {isBoss}) => {aliveEnemies}");
    }

    public void UnregisterEnemy(bool isBoss)
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
        if (isBoss) bossDead = true;   // boss đã rời trận
        // Debug.Log($"[Level2Goal] -1 (boss? {isBoss}) => {aliveEnemies}, bossDead={bossDead}");
        CheckWin();
    }

    /// Gọi ở Animation Event cuối clip chết của boss (nếu bạn muốn chốt thắng ngay khi anim xong)
    public void NotifyBossDead()
    {
        bossPresent = true;
        bossDead    = true;
        CheckWin();
    }
    public void OnBossKilled() => NotifyBossDead(); // alias

    // -------- Core --------
    void CheckWin()
    {
        if (victoryShown) return;

        bool cleared = aliveEnemies <= 0 && (!bossPresent || bossDead);
        if (!cleared) return;

        victoryShown = true;

        if (!victoryUI && autoFindUI) victoryUI = FindObjectOfType<VictoryUI>(true);
        if (victoryUI) victoryUI.Show();      // VictoryUI tự pause Time.timeScale

        OnVictory?.Invoke();
        Debug.Log("[Level2Goal] VICTORY!");
    }
}
