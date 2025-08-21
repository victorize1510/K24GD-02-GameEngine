using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;
    public MyAudio audioManager;
    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<MyAudio>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            audioManager.PlayCoinSound();
            gameManager.AddScore(10);
        }
        else if (collision.CompareTag("Trap") || collision.CompareTag("Enemy") || collision.CompareTag("DeathZone"))
        {
            gameManager.PlayerDied();
        }
    }
}
