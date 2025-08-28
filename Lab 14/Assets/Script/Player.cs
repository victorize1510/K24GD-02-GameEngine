
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using TMPro;
public class Player : MonoBehaviour
{
    public float startY = -4f;
    public float minX, maxX, minY, maxY;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;   // Prefab đạn cơ bản
    public Transform firePoint;       // Vị trí bắn (gắn ở Player)
    public float fireRate = 0.2f;
    private float fireCooldown = 0f;

    [Header("Bullet Upgrade")]
    public int currentBulletLevel = 1;   // Level bắt đầu
    public int maxBulletLevel = 10;      // Level tối đa

    [Header("VFX khi nổ")]
    public GameObject VFX;
    public GameObject shield;
    public int scoreofchickenleg;
    public TMP_Text livesText;
    public int lives = 3; 
    private int currentLives;     
    public Transform respawnPoint; // vị trí respawn (có thể set bằng Inspector)
    private GameOverManager gameOverManager;

    void Start()
    {
        transform.position = new Vector3(0, startY, 0);

        // Tính giới hạn màn hình
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
        minX = bottomLeft.x;
        maxX = topRight.x;
        minY = bottomLeft.y;
        maxY = topRight.y;
        currentLives = lives;
        StartCoroutine(DisabaleShield());
        UpdateLivesUI();
        gameOverManager = FindAnyObjectByType<GameOverManager>();
    }

    void Update()
    {
        MoveWithMouse();
        Fire();
    }

    void MoveWithMouse()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));

        transform.position = new Vector3(
            Mathf.Clamp(worldPos.x, minX, maxX),
            Mathf.Clamp(worldPos.y, minY, maxY),
            0
        );
    }

    void Fire()
    {
        fireCooldown -= Time.deltaTime;
        if (Input.GetMouseButton(0) && fireCooldown <= 0f)
        {
            ShootByLevel();
            fireCooldown = fireRate;
        }
    }

    void ShootByLevel()
    {
        switch (currentBulletLevel)
        {
            case 1: // bắn 1 viên
                Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                break;

            case 2: // bắn 2 viên (chếch nhẹ)
                Instantiate(bulletPrefab, firePoint.position + Vector3.left * 0.15f, Quaternion.identity);
                Instantiate(bulletPrefab, firePoint.position + Vector3.right * 0.15f, Quaternion.identity);
                break;

            case 3: // bắn 3 viên
                Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                Instantiate(bulletPrefab, firePoint.position + Vector3.left * 0.2f, Quaternion.identity);
                Instantiate(bulletPrefab, firePoint.position + Vector3.right * 0.2f, Quaternion.identity);
                break;

            case 4: // bắn 4 viên nhanh
                for (int i = -1; i <= 2; i++)
                    Instantiate(bulletPrefab, firePoint.position + Vector3.right * (i * 0.25f), Quaternion.identity);
                break;

            case 5: // bắn 5 viên (trải rộng hơn)
                for (int i = -2; i <= 2; i++)
                    Instantiate(bulletPrefab, firePoint.position + Vector3.right * (i * 0.2f), Quaternion.identity);
                break;

            case 6: // bắn 3 viên có góc nghiêng
                CreateBulletWithAngle(0);
                CreateBulletWithAngle(15);
                CreateBulletWithAngle(-15);
                break;

            case 7: // bắn 5 viên có góc nghiêng
                CreateBulletWithAngle(0);
                CreateBulletWithAngle(10);
                CreateBulletWithAngle(-10);
                CreateBulletWithAngle(20);
                CreateBulletWithAngle(-20);
                break;

            case 8: // bắn 7 viên (ngang + nghiêng)
                for (int i = -3; i <= 3; i++)
                    CreateBulletWithAngle(i * 5);
                break;

            case 9: // bắn 9 viên
                for (int i = -4; i <= 4; i++)
                    CreateBulletWithAngle(i * 5);
                break;

            case 10: // full power (liên thanh, siêu rộng)
                for (int i = -5; i <= 5; i++)
                    CreateBulletWithAngle(i * 5);
                break;
        }
    }

    void CreateBulletWithAngle(float angle)
    {
        Quaternion rot = Quaternion.Euler(0, 0, angle);
        Instantiate(bulletPrefab, firePoint.position, rot);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gift")) // ăn item nâng cấp
        {
            if (currentBulletLevel < maxBulletLevel)
                currentBulletLevel++;
            Destroy(collision.gameObject);
        }
        else if (!shield.activeSelf && (collision.CompareTag("Chicken") || collision.CompareTag("Egg") || collision.CompareTag("Boss")))
        {
            LoseLife();
        }
        else if (collision.CompareTag("ChickenLeg"))
        {
            Destroy(collision.gameObject);
            ScoreManager.instance.GetScore(scoreofchickenleg);
        }
    }
    IEnumerator DisabaleShield()
    {
        yield return new WaitForSeconds(5);
        shield.SetActive(false);
    }

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded && VFX != null)
        {
            var vfx = Instantiate(VFX, transform.position, Quaternion.identity);
            Destroy(vfx, 1f);
        }
    }
    void LoseLife()
    {
        currentLives--;

        // Hiệu ứng nổ
        if (VFX != null)
        {
            var vfx = Instantiate(VFX, transform.position, Quaternion.identity);
            Destroy(vfx, 1f);
        }

        if (currentLives > 0)
        {
            // Respawn player
            transform.position = respawnPoint != null ? respawnPoint.position : new Vector3(0, -4f, 0);

            // Bật lại shield bảo vệ trong 3s
            shield.SetActive(true);
            StartCoroutine(DisabaleShield());
        }
        else
        {
            // Hết mạng → game over
            Destroy(gameObject);
            if (GameOverManager.instance != null)
            {
                GameOverManager.instance.GameOver();
            }
            Debug.Log("Game Over!");
        }

        UpdateLivesUI();
    }

     void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "x" + currentLives;
        }
    }

}
