using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class GameController : MonoBehaviour
{
    [SerializeField] private Image progressBar;

    public Transform wheel;
    public int segmentCount = 12;
    public float minSpeed = 500f;
    public float maxSpeed = 1000f;
    public float deceleration = 100f;

    public Button spinButton;
    public Text resultText;
    public Text textCooldown;
    public Text totalrubyText;
    int totalruby = 50;

    private bool isSpinning = false;
    private float currentSpeed;

    void Start()
    {
        spinButton.onClick.AddListener(StartSpin);
        Time.timeScale = 1f;
    }

    public void StartSpin()
    {
        if (isSpinning) return;        

        isSpinning = true;
        currentSpeed = Random.Range(minSpeed, maxSpeed);

        totalruby -= 5;
        totalrubyText.text = (totalruby).ToString("F1");

        //currentSpeed = 0f;
        StartCoroutine(SpinCoroutine());
        progressBar.fillAmount = 0f;
        StartCoroutine(CooldownCoroutine());
    }

    IEnumerator SpinCoroutine()
    {
        while (currentSpeed > 0)
        {
            wheel.Rotate(0, 0, -currentSpeed * Time.deltaTime);
            currentSpeed -= deceleration * Time.deltaTime;

            // Ngăn tốc độ âm
            if (currentSpeed < 0)
                currentSpeed = 0;

            yield return null;
        }

        float z = Mathf.Abs(wheel.eulerAngles.z);
        int index = GetRewardIndex(z);
        string reward = GetRewardName(index);

        resultText.text = "Trúng: " + reward;
        Debug.Log("Trúng ô: " + reward);

        isSpinning = false;
    }


    
    private IEnumerator CooldownCoroutine()
    {

        float timer = 10f; // Start the cooldown timer
        while (timer > 0)
        {
            textCooldown.text = "Cooldown: " + (timer).ToString("F1") + "s";
            timer -= Time.deltaTime;
            progressBar.fillAmount = 1f - (timer / 10f); // Update progress bar
            spinButton.interactable = true;
            yield return null;
        }
        spinButton.interactable = false;
    }

    int GetRewardIndex(float z)
    {
        if (z > 0 && z < 30) return 1;
        else if (z > 30 && z < 60) return 2;
        else if (z > 60 && z < 90) return 3;
        else if (z > 90 && z < 120) return 4;
        else if (z > 120 && z < 150) return 5;
        else if (z > 150 && z < 180) return 6;
        else if (z > 180 && z < 210) return 7;
        else if (z > 210 && z < 240) return 8;
        else if (z > 240 && z < 270) return 9;
        else if (z > 270 && z < 300) return 10;
        else if (z > 300 && z < 330) return 11;
        else if (z > 330 && z < 360) return 12;
        return 0;
    }

    string GetRewardName(int index)
    {
        string[] rewards = {
            "12", "01", "02", "03", "04", "05",
            "06", "07", "08", "09", "10", "11", "12"
        };
        return rewards[index];
    }
}


