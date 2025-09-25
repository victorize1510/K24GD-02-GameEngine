using UnityEngine;
using System;

[DefaultExecutionOrder(-100)]                 // khởi tạo sớm để UI subscribe kịp
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] int score;               // xem realtime trên Inspector
    public int Score => score;

    public event Action<int> OnScoreChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Broadcast();                           // phát giá trị ban đầu (0) cho UI
        DontDestroyOnLoad(gameObject);      // bật nếu muốn giữ qua scene
    }

    [ContextMenu("Add 1 (debug)")]            // click trong Inspector để test
    public void AddOneDebug() => Add(1);

    public void ResetScore()
    {
        score = 0;
        Broadcast();
    }

    public void Add(int amount)
    {
        score = Mathf.Max(0, score + amount);
        Debug.Log($"[ScoreManager] +{amount} => {score}");
        Broadcast();
    }

    void Broadcast() => OnScoreChanged?.Invoke(score);
}