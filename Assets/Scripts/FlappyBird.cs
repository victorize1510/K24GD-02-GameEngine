using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlappyBird : MonoBehaviour
{
    public Rigidbody2D rigibody;
    public float jumpForce;
    private bool levelStart;
    public GameObject GameController;
    public GameObject Menu;
    public GameObject GameOverPanel;
    public GameObject ScoreDisplay;
    public GameObject cam;
    public Text bestScoreText;
    private int score;
    private bool isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        ScoreDisplay.SetActive(false);
        rigibody = this.gameObject.GetComponent<Rigidbody2D>();
        levelStart = false;
        rigibody.gravityScale = 0;
        score = 0;
        GameController.GetComponent<ScoreManager>().UpdateScore(score);
        Menu.GetComponent<SpriteRenderer>().enabled = true;// trước khi bắt đầu game sẽ hiện hướng dẫn menu
        GameOverPanel.SetActive(false); // Ẩn UI Game Over khi bắt đầu
    }
    void StartGame()
    {
        ScoreDisplay.SetActive(true); // hiện điểm
    }


    // Update is called once per frame
    void Update()
    {
        // Nếu chim chết rồi thì không điều khiển nữa
        if (isDead)
            return;
        //Chỉnh góc độ xoay con chim
        float angle = Mathf.Clamp(rigibody.velocity.y * 3f, -90f, 30f);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (levelStart == false)
            {
                levelStart = true;
                rigibody.gravityScale = 2;
                GameController.GetComponent<PipeGenerator>().enableGenratePipe = true;
                Menu.GetComponent<SpriteRenderer>().enabled = false;
                StartGame();
            }
            BirdMoveUp();
        }
    }
    private void BirdMoveUp() //lam chim bay len 1 khoang
    {
        rigibody.linearVelocity = Vector2.up * jumpForce;
        Soundcontroller.instance.PlaySound("wing");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        isDead = true; // đánh dấu chim đã chết
        Soundcontroller.instance.PlaySound("hit");
        score = 0;
        GameController.GetComponent<ScoreManager>().UpdateScore(score);
        // Ngừng pipe spawn
        GameController.GetComponent<PipeGenerator>().enableGenratePipe = false;
        // Dừng chim
        rigibody.velocity = Vector2.zero;
        rigibody.gravityScale = 0;
        

        StartCoroutine(Die());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;
        Soundcontroller.instance.PlaySound("point");
        score += 1;
        GameController.GetComponent<ScoreManager>().UpdateScore(score);
        int best = PlayerPrefs.GetInt("BestScore", 0);
        if (score > best)
        {
            PlayerPrefs.SetInt("BestScore", score);
            PlayerPrefs.Save(); // lưu dữ liệu
        }
    }
    public void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
    }
    int n = 5;
    void ShakeCam()
    {
        n = (n == 5) ? -5 : 5; 
        cam.transform.rotation = Quaternion.Euler(0,0,n);
    }    
    IEnumerator Die()
    {
        InvokeRepeating("ShakeCam", 0, 0.025f);
        yield return new WaitForSeconds(0.1f);
        CancelInvoke();
        cam.transform.rotation = Quaternion.Euler(0, 0, 0);
        int best = PlayerPrefs.GetInt("BestScore", 0);
        bestScoreText.text = "BestScore: " + best.ToString();
        GameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }
}
    