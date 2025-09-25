using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class SkeletonYellowController : MonoBehaviour,IDamageable
{
   [Header("Patrol")]
    public Transform pointA, pointB;
    public float moveSpeed = 2f;
    public float arriveThreshold = 0.25f;
    public float waitAtPoint = 0.3f;

    [Header("Detect (ground/wall)")]
    public LayerMask groundMask;
    public float wallCheckDistance = 0.3f;
    public float ledgeCheckDistance = 0.35f;
    public Transform feet;

    [Header("Chase/Combat (only after hit)")]
    public Transform player;
    public float chaseSpeed = 2.5f;
    public float stopChaseRange = 12f;
    public float attackRange = 1.2f;     // theo X
    public float attackCooldown = 1.0f;
    public int   contactDamage = 1;

    [Tooltip("Tỉ lệ chồng lấp theo trục Y giữa collider (0.25 = 25%).")]
    [Range(0f, 1f)] public float yOverlapPercent = 0.25f;

    [Header("Stats")]
    public int hp = 10;

    [Header("Death")]
    public bool destroyByDelay = true;
    public float deathDestroyDelay = 2.0f;
    public bool disableCollidersOnDeath = true;

    [Header("Debug")]
    public bool debugLogs = true;
    public bool drawRays = true;

    [Header("Combat Tuning")]
    public float counterDelayOnHit = 0.12f;  // NEW: trễ phản đòn sau khi bị đánh
    float nextAttackAllowedAt = -999f;       // NEW: thời điểm cho phép ra đòn lại

    // --- Private ---
    Animator anim;
    Rigidbody2D rb;
    Collider2D colSelf, colPlayer;
    Transform currentTarget;
    bool isDead, isWaiting;
    float startY;

    bool isAggro = false;           
    float lastAttackTime = -999f;   
    bool attackWindow = false;      
    bool hasHitThisSwing = false;   
    bool facingRight = true;

    float stuckTimer = 0f;
    Vector2 lastPos;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb   = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        colSelf = GetComponentInChildren<Collider2D>();
    }

    void Start()
    {
        if (!pointA || !pointB) { Debug.LogWarning("Chưa gán pointA/pointB"); enabled = false; return; }
        if (!feet) feet = transform;

        if (!player) { var p = GameObject.FindGameObjectWithTag("Player"); if (p) player = p.transform; }
        if (player) colPlayer = player.GetComponentInChildren<Collider2D>();

        currentTarget = pointB;
        startY = transform.position.y;
        lastPos = transform.position;
    }

    void FixedUpdate()
    {
        if (isDead || isWaiting) return;
        if (isAggro && player) ChaseLogic(); else PatrolLogic();
    }

    // ===== Patrol =====
    void PatrolLogic()
    {
        float dirX = Mathf.Sign(currentTarget.position.x - transform.position.x);

        Vector2 wallOrigin  = new Vector2(transform.position.x, feet.position.y);
        Vector2 ledgeOrigin = new Vector2(transform.position.x + dirX * ledgeCheckDistance, feet.position.y);

        var wallHit     = Physics2D.Raycast(wallOrigin,  new Vector2(dirX, 0f), wallCheckDistance, groundMask);
        var groundAhead = Physics2D.Raycast(ledgeOrigin, Vector2.down, 1.0f,   groundMask);

        if (drawRays) {
            Debug.DrawRay(wallOrigin,  new Vector2(dirX, 0f) * wallCheckDistance, Color.yellow);
            Debug.DrawRay(ledgeOrigin, Vector2.down * 1.0f,                       Color.magenta);
        }

        if (wallHit.collider != null || groundAhead.collider == null)
        { StartCoroutine(SwapPointAfterWait()); return; }

        Vector2 pos = rb.position;
        Vector2 target = new Vector2(currentTarget.position.x, startY);
        Vector2 next = Vector2.MoveTowards(pos, target, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        if (dirX > 0.01f) SetFacingRight(true);
        else if (dirX < -0.01f) SetFacingRight(false);

        anim.SetBool("IsWalking", Mathf.Abs(next.x - pos.x) > 0.0001f);

        if (Mathf.Abs(next.x - target.x) <= arriveThreshold)
            StartCoroutine(SwapPointAfterWait());

        Watchdog(dirX);
    }

    // ===== Chase (chỉ khi đã aggro) =====
    void ChaseLogic()
    {
        float deltaX = player.position.x - transform.position.x;
        float dirX = Mathf.Sign(deltaX);

        Vector2 wallOrigin  = new Vector2(transform.position.x, feet.position.y);
        Vector2 ledgeOrigin = new Vector2(transform.position.x + dirX * ledgeCheckDistance, feet.position.y);

        var wallHit     = Physics2D.Raycast(wallOrigin,  new Vector2(dirX, 0f), wallCheckDistance, groundMask);
        var groundAhead = Physics2D.Raycast(ledgeOrigin, Vector2.down, 1.0f,   groundMask);

        if (drawRays) {
            Debug.DrawRay(wallOrigin,  new Vector2(dirX, 0f) * wallCheckDistance, Color.cyan);
            Debug.DrawRay(ledgeOrigin, Vector2.down * 1.0f,                       Color.blue);
        }

        if (wallHit.collider != null || groundAhead.collider == null)
        { isAggro = false; return; }

        float distX = Mathf.Abs(deltaX);
        if (distX > stopChaseRange) { isAggro = false; return; }

        // Vào tầm X, hết cooldown và qua thời điểm cho phép → tấn công
        if (distX <= attackRange
            && Time.time >= lastAttackTime + attackCooldown
            && Time.time >= nextAttackAllowedAt)                 // NEW
        {
            lastAttackTime = Time.time;
            hasHitThisSwing = false;
            anim.ResetTrigger("Attack");
            anim.SetTrigger("Attack");
            anim.SetBool("IsWalking", false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // áp sát
        Vector2 pos = rb.position;
        Vector2 target = new Vector2(player.position.x, startY);
        Vector2 next = Vector2.MoveTowards(pos, target, chaseSpeed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        if (dirX > 0.01f) SetFacingRight(true);
        else if (dirX < -0.01f) SetFacingRight(false);

        anim.SetBool("IsWalking", Mathf.Abs(next.x - pos.x) > 0.0001f);

        Watchdog(dirX);

        // chỉ gây dame trong cửa sổ cho phép
        if (attackWindow) TryHitPlayer();
    }

    void Watchdog(float dirX)
    {
        float moved = Mathf.Abs(transform.position.x - lastPos.x);
        if (moved < 0.003f) stuckTimer += Time.fixedDeltaTime;
        else { stuckTimer = 0f; lastPos = transform.position; }

        if (stuckTimer > 1.0f) { StartCoroutine(SwapPointAfterWait()); stuckTimer = 0f; lastPos = transform.position; isAggro = false; }
    }

    IEnumerator SwapPointAfterWait()
    {
        isWaiting = true;
        anim.SetBool("IsWalking", false);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(waitAtPoint);
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
        isWaiting = false;
    }

    void SetFacingRight(bool right)
    {
        facingRight = right;
        var s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (right ? 1f : -1f);
        transform.localScale = s;
    }

    // ===== BỊ Player GÂY DAME =====
    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        hp -= dmg;
        if (debugLogs) Debug.Log($"[Skel] TakeDamage {dmg}, hp={hp}");

        // Khi vừa bị đánh: aggro + khóa phản đòn ngay frame này
        if (player) isAggro = true;
        nextAttackAllowedAt = Time.time + counterDelayOnHit;   // NEW: trễ phản đòn
        hasHitThisSwing = false;                                // NEW
        attackWindow = false;                                   // NEW
        anim.ResetTrigger("Attack");                            // NEW: huỷ trigger tấn công đang chờ
        anim.CrossFade("SkeletonYellow_Hurt", 0f, 0);

        if (hp <= 0) { Die(); return; }
    }

    // ===== Điều kiện hit không dùng hitbox =====
    bool IsFacingPlayer()
    {
        float dx = player.position.x - transform.position.x;
        return Mathf.Sign(dx) == (facingRight ? 1f : -1f);
    }
    bool AttackXInRange() => Mathf.Abs(player.position.x - transform.position.x) <= attackRange;

    bool AttackYOverlap()
    {
        if (!colSelf || !colPlayer) return true;
        Bounds a = colSelf.bounds, b = colPlayer.bounds;
        float overlap = Mathf.Min(a.max.y, b.max.y) - Mathf.Max(a.min.y, b.min.y);
        float need = Mathf.Min(a.size.y, b.size.y) * yOverlapPercent;
        return overlap >= need;
    }

    // ===== Skeleton gây sát thương: chỉ trong AttackWindow =====
    void TryHitPlayer()
    {
        if (!player) return;
        if (hasHitThisSwing) return;
        if (Time.time < nextAttackAllowedAt) return;           // NEW: đang hit-stun → không gây dame

        bool facingOk = IsFacingPlayer();
        bool xOk = AttackXInRange();
        bool yOk = AttackYOverlap();
        if (!facingOk || !xOk || !yOk) return;

        var dmgable = player.GetComponentInParent<IDamageable>();
        if (dmgable == null) return;

        dmgable.TakeDamage(contactDamage);
        hasHitThisSwing = true;
        if (debugLogs) Debug.Log($"[Skel] HIT PLAYER for {contactDamage}");
    }

    // ===== Death =====
    void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("IsDead", true);

        if (disableCollidersOnDeath)
            foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;

        if (destroyByDelay) StartCoroutine(DestroyAfterDelay(deathDestroyDelay));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    public void OnDeathEnd() => Destroy(gameObject);

    // ===== Animation Events trong clip Attack =====
    public void AttackWindow_On()
    {
        if (Time.time < nextAttackAllowedAt) {                 // NEW: còn hit-stun → không mở cửa sổ
            if (debugLogs) Debug.Log("[Skel] AttackWindow_On blocked by hit-stun");
            return;
        }
        attackWindow = true;
        TryHitPlayer();
    }
    public void AttackWindow_Off()
    {
        attackWindow = false;
    }
}
