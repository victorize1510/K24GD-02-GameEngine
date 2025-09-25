using UnityEngine;

public class EnemyRegister : MonoBehaviour
{
    bool registered = false;

    void OnEnable()  { TryRegister(); }
    void Start()     { TryRegister(); }

    void TryRegister()
    {
        if (registered) return;
        if (LevelGoal.Instance != null)
        {
            LevelGoal.Instance.RegisterEnemy();
            registered = true;
        }
    }

    void OnDestroy()
    {
        if (registered && LevelGoal.Instance != null)
            LevelGoal.Instance.UnregisterEnemy();
    }
}