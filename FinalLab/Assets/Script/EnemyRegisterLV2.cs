using UnityEngine;

public class EnemyRegisterLV2 : MonoBehaviour
{
    [Tooltip("Đánh dấu là Boss để Level2Goal biết khi boss chết thì có thể chốt thắng.")]
    public bool isBoss = false;

    bool registered = false;
    bool quitting   = false;

    void OnEnable()  { TryRegister(); }
    void Start()     { TryRegister(); }

    void TryRegister()
    {
        if (registered) return;
        if (Level2Goal.Instance == null) return;

        Level2Goal.Instance.RegisterEnemy(isBoss);
        registered = true;
        // Debug.Log($"[EnemyRegister] Registered ({name}) isBoss={isBoss}");
    }

    void OnDestroy()
    {
        if (quitting) return; // thoát Play Mode thì bỏ qua
        TryUnregister();
    }
    void OnApplicationQuit() => quitting = true;

    void TryUnregister()
    {
        if (!registered) return;
        if (Level2Goal.Instance == null) return;

        Level2Goal.Instance.UnregisterEnemy(isBoss);
        registered = false;
        // Debug.Log($"[EnemyRegister] Unregistered ({name}) isBoss={isBoss}");
    }
}
