using UnityEngine;
using TMPro;

public class CoinsTextTMP : MonoBehaviour
{
    public TMP_Text tmp;
    public string prefix = "x ";

    void Awake()
    {
        if (!tmp) tmp = GetComponent<TMP_Text>();
        if (tmp) tmp.text = prefix + "0";   // hiện 0 ngay từ đầu
    }

    void OnEnable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateText;
            UpdateText(ScoreManager.Instance.Score);
        }
        else
        {
            Debug.LogWarning("[CoinsTextTMP] Chưa có ScoreManager trong scene.");
        }
    }

    void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= UpdateText;
    }

    void UpdateText(int score)
    {
        if (tmp) tmp.text = prefix + score;
    }
}
