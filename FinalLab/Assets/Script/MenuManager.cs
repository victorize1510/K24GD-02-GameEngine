using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainPanel, optionsPanel, tutorialPanel;
    public string level1SceneName = "Level 1";

    void Reset() { AutoFind(); }
    void Awake()
    {
        if (!mainPanel || !optionsPanel || !tutorialPanel)
            AutoFind();

        ShowMain(); // mặc định: chỉ MainPanel mở
    }

    void AutoFind()
    {
        Transform t = transform;
        mainPanel    = mainPanel    ? mainPanel    : t.Find("MainPanel")?.gameObject;
        optionsPanel = optionsPanel ? optionsPanel : t.Find("OptionsPanel")?.gameObject;
        tutorialPanel= tutorialPanel? tutorialPanel: t.Find("TutorialPanel")?.gameObject;
    }

    public void ShowMain()
    {
        if (mainPanel)    mainPanel.SetActive(true);
        if (optionsPanel) optionsPanel.SetActive(false);
        if (tutorialPanel)tutorialPanel.SetActive(false);
    }

    public void Play()
    {
        // đảm bảo unpause
        Time.timeScale = 1f;

        // new run -> điểm về 0
        if (ScoreManager.Instance) ScoreManager.Instance.ResetScore();

        // heal Player ngay sau khi load Level 1
        SceneManager.sceneLoaded += HealPlayerAfterLoad;
        SceneManager.LoadScene(level1SceneName);
    }
    void HealPlayerAfterLoad(Scene s, LoadSceneMode m)
    {
        SceneManager.sceneLoaded -= HealPlayerAfterLoad;

        var player = FindObjectOfType<PlayerController>(true);
        if (player) player.FullHeal();
    }
    public void OpenOptions()
    { if (optionsPanel) { optionsPanel.SetActive(true); mainPanel?.SetActive(false); tutorialPanel?.SetActive(false); } }
    public void CloseOptions()   => ShowMain();
    public void OpenTutorial()
    { if (tutorialPanel){ tutorialPanel.SetActive(true); mainPanel?.SetActive(false); optionsPanel?.SetActive(false);} }
    public void CloseTutorial()  => ShowMain();
}