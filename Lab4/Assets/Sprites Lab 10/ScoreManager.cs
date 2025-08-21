using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;
public class GameManager : MonoBehaviour
{
    private int Score = 0;
    public TMP_Text scoreText;
    public GameObject Gameover;
    private bool isGameOver = false;
    public TMP_Text gameOverScoreText;
    public int lives = 3;
    public TMP_Text lifeText;          // Text hiển thị số mạng
    public GameObject playerPrefab;         // Prefab Player
    private GameObject currentPlayer;
    public Transform respawnPoint;
    public CinemachineCamera cineCam;  
    void Start()
    {
        currentPlayer = GameObject.FindWithTag("Player");
        UpdateLives();
        UpdateScore();
        Gameover.SetActive(false);
        // Nếu muốn an toàn hơn thì check null
        if (cineCam != null && currentPlayer != null)
        {
            cineCam.Follow = currentPlayer.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AddScore(int points)
    {
        if (!isGameOver)
        {
            Score += points;
            UpdateScore();
        }
    }
    public void UpdateScore()
    {
        scoreText.text = "Score: " + Score.ToString();
    }
    public void GameOver()
    {
        isGameOver = true;
        if (gameOverScoreText != null)
            gameOverScoreText.text = "Score: " + Score.ToString();
        Time.timeScale = 0;
        Gameover.SetActive(true);
    }
    public void RestartGame()
    {
        isGameOver = false;
        Time.timeScale = 1;
        lives = 3;
        UpdateScore();
        UpdateLives();
        Score = 0;
        SceneManager.LoadScene("Both");
    }
    public bool IsGameOver()
    {
        return isGameOver;
    }
    void UpdateLives()
    {
        lifeText.text = "x" + lives.ToString();
    }
     public void PlayerDied()
    {
        if (isGameOver) return;

    lives--;
    UpdateLives();

    if (lives > 0)
    {
        // Xóa player cũ trước khi respawn
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        SpawnPlayer();
    }
    else
    {
        // Xóa player cuối cùng rồi game over
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }
        GameOver();
    }
    }
    void SpawnPlayer()
    {
        Vector3 spawnPos = respawnPoint.position + Vector3.up * 1.5f;
        currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        if (cineCam != null)
        {
            cineCam.Follow = currentPlayer.transform;
        }
    }
}

