using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class HealthUI : MonoBehaviour
{
     public TMP_Text healthText; 
    public PlayerController player; 

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Rebind();
        Refresh();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        Refresh(); 
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        Rebind();
        Refresh();
    }

    void Rebind()
    {
        if (!player) player = FindObjectOfType<PlayerController>();
    }

    void Refresh()
    {
        if (healthText && player)
            healthText.text = $"Health: {player.CurrentHP}";
    }
}
