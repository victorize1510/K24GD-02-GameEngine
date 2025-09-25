using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class BossController : MonoBehaviour, IDamageable
{
    [Header("Refs")]
    public Animator animator;
    public Rigidbody2D rb;
    public Transform player;                    // để trống sẽ tự FindWithTag("Player")

    [Header("Stats")]
    public int maxHP = 80;
    public int contactDamage = 2;

    [Header("Move / Chase")]
    public float moveSpeed = 2.4f;
    public float aggroRange = 12f;              // thấy player và bắt đầu đuổi
    public float stopChaseRange = 16f;          // ra ngoài tầm này thì ngừng đuổi

    [Header("Attack")]
    public float attackRangeX = 1.6f;           // khoảng cách X để ra đòn
    public float attackCooldown = 1.1f;         // hồi chiêu
    [Range(0f,1f)] public float yOverlapPercent = 0.25f; // % chồng lấp theo chiều cao để tính “cùng tầng”

    [Header("Death")]
    public bool disableCollidersOnDeath = true;
    public bool destroyAfterDeath = true;
    public float destroyDelay = 2f;

    [Header("Optional Win Hook")]
    public Level2Goal level2Goal;               // (tùy chọn) kéo Level2Goal vào để báo thắng

    // ---- private
    int hp;
    bool dead = false;
    bool facingRight = true;
    bool inAttackWindow = false;
    bool hasHitThisSwing = false;
    float nextAttackAllowedAt = 0f;

    Collider2D myCol, playerCol;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!rb) rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        if (!player)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
        myCol = GetComponentInChildren<Collider2D>();
        if (player) playerCol = player.GetComponentInChildren<Collider2D>();

        if (!level2Goal) level2Goal = FindObjectOfType<Level2Goal>(true);

        hp = maxHP;
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hurt");
        animator.SetBool("IsDead", false);
    }

    void Update()
    {
        if (dead || !player) { animator.SetBool("IsWalking", false); return; }

        float dx = player.position.x - transform.position.x;
        float absX = Mathf.Abs(dx);

        // Luôn CHỦ ĐỘNG tấn công khi đủ điều kiện
        if (absX <= attackRangeX && Time.time >= nextAttackAllowedAt && YOverlap())
        {
            FaceByDir(dx);
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
            animator.SetBool("IsWalking", false);
            rb.linearVelocity = Vector2.zero;
            nextAttackAllowedAt = Time.time + attackCooldown;
            return;
        }

        // Chủ động rượt khi lọt vào tầm aggro, và tiếp tục rượt cho tới khi ra khỏi stopChaseRange
        bool keepChasing = animator.GetBool("IsWalking") && absX <= stopChaseRange;
        if (absX <= aggroRange || keepChasing)
        {
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 next = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.deltaTime);
            rb.MovePosition(next);

            animator.SetBool("IsWalking", Mathf.Abs(next.x - rb.position.x) > 0.0001f);
            FaceByDir(dx);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        if (inAttackWindow) TryHitPlayer(); // khi đang mở cửa sổ đánh, thử gây dame
    }

    void FaceByDir(float dx)
    {
        bool wantRight = dx >= 0f;
        if (wantRight != facingRight)
        {
            facingRight = wantRight;
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (facingRight ? 1f : -1f);
            transform.localScale = s;
        }
    }

    bool YOverlap()
    {
        if (!myCol || !playerCol) return true;
        Bounds a = myCol.bounds, b = playerCol.bounds;
        float overlap = Mathf.Min(a.max.y, b.max.y) - Mathf.Max(a.min.y, b.min.y);
        float need = Mathf.Min(a.size.y, b.size.y) * yOverlapPercent;
        return overlap >= need;
    }

    // ================= IDamageable =================
    public void TakeDamage(int dmg)
    {
        if (dead) return;

        int before = hp;
        hp = Mathf.Max(0, hp - dmg);
        // Debug.Log($"[Boss] {before}->{hp}");

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Hurt");
        }
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        animator.SetBool("IsDead", true);
        animator.SetBool("IsWalking", false);
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        if (disableCollidersOnDeath)
            foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;
    }

    // ========== Animation Events ==========
    public void AttackWindow_On()
    {
        inAttackWindow = true;
        hasHitThisSwing = false;
        TryHitPlayer();
    }

    public void AttackWindow_Off()
    {
        inAttackWindow = false;
    }

    void TryHitPlayer()
    {
        if (!inAttackWindow || hasHitThisSwing || !player) return;
        if (!YOverlap()) return;

        float dx = player.position.x - transform.position.x;
        if (Mathf.Sign(dx) != (facingRight ? 1f : -1f)) return;
        if (Mathf.Abs(dx) > attackRangeX) return;

        var dmgable = player.GetComponentInParent<IDamageable>();
        if (dmgable != null)
        {
            dmgable.TakeDamage(contactDamage);
            hasHitThisSwing = true;
            // Debug.Log($"[Boss] hit player {contactDamage}");
        }
    }

    // Cuối clip death
    public void OnDeathEnd()
    {
        
        if (!dead) {        // <-- chốt quan trọng
            Debug.LogWarning("[Boss] OnDeathEnd được gọi khi chưa chết. Kiểm tra Animation Events/AnyState->Death!");
            return;
        }
        if (level2Goal) level2Goal.OnBossKilled();   // hoặc NotifyBossDead() tùy Goal script bạn dùng
        if (destroyAfterDeath) Destroy(gameObject);
        }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.left * attackRangeX,
                        transform.position + Vector3.right * attackRangeX);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.left * aggroRange,
                        transform.position + Vector3.right * aggroRange);
    }
}
