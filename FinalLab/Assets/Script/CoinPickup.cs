using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    public int value = 1;
    public bool destroyOnPickup = true;

    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true; // đảm bảo là trigger
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log($"[Coin] picked by: {other.name}");

        if (ScoreManager.Instance == null)
        {
            Debug.LogWarning("[Coin] No ScoreManager in scene!");
            return;
        }

        ScoreManager.Instance.Add(value);
        SoundManager.Instance?.PlayCoin();

        if (destroyOnPickup) Destroy(gameObject);
        else col.enabled = false; // tránh ăn lại
    }
}
