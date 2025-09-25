using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;     // Panel có Resume/Options/Main Menu
    public GameObject optionsPanel;   // Panel có 2 slider

    [Header("Buttons")]
    public Button pauseHotkeyButton;  // (tuỳ chọn) Nút pause trên HUD
    public Button resumeButton;
    public Button optionsButton;
    public Button closeOptionsButton; // nút X trên Options
    public Button mainMenuButton;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Text musicPercent;     // (tuỳ chọn) hiển thị %
    public TMP_Text sfxPercent;

    [Header("Behaviour")]
    [SerializeField] string mainMenuScene = "MainMenu";
    [Tooltip("Bật nếu PauseCanvas (hoặc object này) có DontDestroyOnLoad; " +
             "khi về MainMenu sẽ tự huỷ để không đè UI menu.")]
    public bool destroyOnMainMenu = true;

    float lastTimeScale = 1f;
    bool isPaused = false;

    void Awake()
    {
        // Ẩn panel lúc đầu
        if (pausePanel)   pausePanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);

        // Buttons
        if (pauseHotkeyButton) pauseHotkeyButton.onClick.AddListener(TogglePause);
        if (resumeButton)      resumeButton.onClick.AddListener(Resume);
        if (optionsButton)     optionsButton.onClick.AddListener(OpenOptions);
        if (closeOptionsButton)closeOptionsButton.onClick.AddListener(CloseOptions);
        if (mainMenuButton)    mainMenuButton.onClick.AddListener(BackToMainMenu);

        // Sliders (0..1) + lắng nghe
        if (musicSlider)
        {
            musicSlider.minValue = 0f; musicSlider.maxValue = 1f; musicSlider.wholeNumbers = false;
            musicSlider.onValueChanged.AddListener(v =>
            {
                SoundManager.Instance?.SetMusicVolume(v);
                if (musicPercent) musicPercent.text = Mathf.RoundToInt(v * 100f) + "%";
            });
        }
        if (sfxSlider)
        {
            sfxSlider.minValue = 0f; sfxSlider.maxValue = 1f; sfxSlider.wholeNumbers = false;
            sfxSlider.onValueChanged.AddListener(v =>
            {
                SoundManager.Instance?.SetSfxVolume(v);
                if (sfxPercent) sfxPercent.text = Mathf.RoundToInt(v * 100f) + "%";
            });
        }
    }

    void Start() => SyncSlidersFromManager();

    void Update()
    {
        // Phím tắt pause
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            TogglePause();
    }

    // ========= Pause/Resume =========
    public void TogglePause() { if (!isPaused) Pause(); else Resume(); }

    public void Pause()
    {
        isPaused = true;
        if (pausePanel)   pausePanel.SetActive(true);
        if (optionsPanel) optionsPanel.SetActive(false);

        // Ẩn nút Pause trên HUD để tránh click nhầm khi đang tạm dừng
        if (pauseHotkeyButton) pauseHotkeyButton.gameObject.SetActive(false);

        lastTimeScale = Time.timeScale;
        Time.timeScale = 0f; // dừng game

        SyncSlidersFromManager();
    }

    public void Resume()
    {
        isPaused = false;
        if (pausePanel)   pausePanel.SetActive(false);
        if (optionsPanel) optionsPanel.SetActive(false);

        // Hiện lại nút Pause trên HUD
        if (pauseHotkeyButton) pauseHotkeyButton.gameObject.SetActive(true);

        Time.timeScale = lastTimeScale <= 0f ? 1f : lastTimeScale;
    }

    // ========= Options =========
    public void OpenOptions()
    {
        SyncSlidersFromManager();
        if (optionsPanel) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel) optionsPanel.SetActive(false);
    }

    void SyncSlidersFromManager()
    {
        if (!SoundManager.Instance) return;

        float mv = Mathf.Clamp01(SoundManager.Instance.musicVolume);
        float sv = Mathf.Clamp01(SoundManager.Instance.sfxVolume);

        if (musicSlider)
        {
            musicSlider.SetValueWithoutNotify(mv);
            if (musicPercent) musicPercent.text = Mathf.RoundToInt(mv * 100f) + "%";
        }
        if (sfxSlider)
        {
            sfxSlider.SetValueWithoutNotify(sv);
            if (sfxPercent) sfxPercent.text = Mathf.RoundToInt(sv * 100f) + "%";
        }
    }

    // ========= Main Menu =========
    public void BackToMainMenu()
    {
        // Bỏ pause & tắt toàn bộ panel trước khi đổi scene
        isPaused = false;
        Time.timeScale = 1f;

        if (optionsPanel) optionsPanel.SetActive(false);
        if (pausePanel)   pausePanel.SetActive(false);
        if (pauseHotkeyButton) pauseHotkeyButton.gameObject.SetActive(false);
        // reset điểm cho run mới
        if (ScoreManager.Instance) ScoreManager.Instance.ResetScore();
        if (destroyOnMainMenu)
        {
            StartCoroutine(LoadMainThenDispose());
        }
        else
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }

    System.Collections.IEnumerator LoadMainThenDispose()
    {
        SceneManager.LoadScene(mainMenuScene);
        yield return null; // đợi 1 frame để scene vào xong
        // Huỷ root (PauseCanvas) để không dính UI pause ở Main Menu
        Destroy(transform.root.gameObject);
    }
}