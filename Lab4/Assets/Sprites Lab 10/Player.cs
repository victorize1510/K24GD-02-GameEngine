using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gameManager;
    public MyAudio audioManager;
    public Animator animator;
    public Rigidbody2D rb;
    public float moveSpeed = 3f;
    public float jumpForce = 5f;
    public SpriteRenderer spriteRenderer;
    private bool isGrounded;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float bounceForce = 8f;

    // Thêm layer riêng cho Platform Effector
    public LayerMask platformLayer;
    // Lưu Rigidbody của platform đứng trên
    private Rigidbody2D currentPlatformRb;
    private MovingPlatform currentPlatform;
    public float extraJumpForce = 2f;
    private Vector3 spawnPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.freezeRotation = true;
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<MyAudio>();
        spawnPoint = transform.position;
    }
     public void Respawn()
    {
        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPoint; // 🔹 Đặt lại vị trí spawn
    }


    void Update()
    {
        // Input di chuyển
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        // Lật sprite
        if (moveInput > 0) spriteRenderer.flipX = false;
        else if (moveInput < 0) spriteRenderer.flipX = true;

        // Kiểm tra grounded
        bool groundCheckNormal = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        bool groundCheckPlatform = Physics2D.OverlapCircle(groundCheck.position, 0.2f, platformLayer);

        // Nếu đứng trên đất thường hoặc platform effector thì coi như grounded
        isGrounded = groundCheckNormal || groundCheckPlatform;

        // Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            audioManager.PlayJumpSound();
            // Lấy lực bổ sung từ platform
            float extraForce = 0f;
            if (currentPlatform != null)
                extraForce = currentPlatform.extraJumpForce; // hoặc currentPlatformRb.velocity.y nếu muốn lực theo Y platform

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce + extraForce);
        }

        UpdateAnimation();
        if (gameManager.IsGameOver()) return;
    }

    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJump = !isGrounded;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJump", isJump);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyHead"))
        {
            // Gọi hàm Die() bên Enemy
            Enemy enemy = collision.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.Die();
            }
            // Player bật lên lại (giống Mario)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatformRb = collision.gameObject.GetComponent<Rigidbody2D>();
            currentPlatform = collision.gameObject.GetComponent<MovingPlatform>();
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy touched (body)!");
            gameManager.PlayerDied();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatformRb = null;
            currentPlatform = null;
        }
    }

}