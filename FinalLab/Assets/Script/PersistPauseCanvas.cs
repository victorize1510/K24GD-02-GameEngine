using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-500)]
public class PersistPauseCanvas : MonoBehaviour
{
    static PersistPauseCanvas instance;
    Canvas canvas;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        canvas = GetComponentInChildren<Canvas>(true);
        SceneManager.activeSceneChanged += (_, __) => RebindCamera();
        RebindCamera();
    }

    void RebindCamera()
    {
        if (!canvas) return;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            canvas.worldCamera = Camera.main; // gán camera của scene mới
        canvas.sortingOrder = 9999;          // luôn nổi trên cùng
        gameObject.SetActive(true);
    }
}
