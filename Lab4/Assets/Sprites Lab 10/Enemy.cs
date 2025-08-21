using UnityEngine;
public class Enemy : MonoBehaviour
{ private Rigidbody2D rb;
    private Animator animator;
    private bool isDead = false;

    public float fallGravity = 3f;

    // Cho EnemyMove truy cập trạng thái chết
    public bool IsDead { get { return isDead; } }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Chuyển animation Die
        animator.SetTrigger("Die");

        // Dừng di chuyển
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = fallGravity;

        // Tắt collider để không còn va chạm
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Xoá enemy sau 2 giây
        Destroy(gameObject, 2f);
    }
}
