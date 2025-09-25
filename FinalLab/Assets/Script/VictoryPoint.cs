using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class VictoryPoint : MonoBehaviour
{
    void Awake() { GetComponent<Collider2D>().isTrigger = true; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (LevelGoal.Instance != null)
            LevelGoal.Instance.TryWin();
        else
            Debug.LogWarning("[VictoryPoint] Chưa có LevelGoal trong scene.");
    }
}