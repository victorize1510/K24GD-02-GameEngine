using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] string dieTrigger = "Die";   // trigger để vào state Death
    [SerializeField] string dieState   = "Player_Death";

    [Header("Refs")]
    public Animator animator;
    public Rigidbody2D rb;
    private SlopeMove slope;

    [Header("Move")]
    public float moveSpeed = 5f;
    public float jumpHeight = 15f;
    public bool isGround = true;

    [Header("Combat")]
    public int maxHP = 50;
    public float invulnTime = 0.6f;
    public int attackDamage = 1;

    [Header("Attack Area")]
    public Transform attackPoint;
    public Vector2 attackSize = new Vector2(1.2f, 1.0f);
    public LayerMask enemyMask;

    [Header("Death / Fail")]
    public bool killByFallY = true;
    public float killY = -6f;
    public string animDeadBool = "Die";
    public GameOverUI gameOverUI; // sẽ auto-find nếu để trống

    [Header("Debug UI")]
    public bool showSimpleHpText = true;

    [SerializeField] private int hp;
    private float movement;
    private bool facingRight = true;
    private bool invuln = false;
    private bool dead = false;

    private bool attackWindow = false;
    private readonly HashSet<int> hitThisSwing = new HashSet<int>();

    // ========= LIFECYCLE =========
    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!rb) rb = GetComponent<Rigidbody2D>();
        slope = GetComponent<SlopeMove>();

        if (!gameOverUI) gameOverUI = FindObjectOfType<GameOverUI>(true);
    }

    void OnEnable()
    {
        // Nếu Player được giữ lại qua scene thì vẫn heal
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        FullHeal();
    }

    void Start()
    {
        // Nếu Player KHÔNG persist thì Start sẽ chạy mỗi scene -> heal luôn
        FullHeal();
        Debug.Log($"[Player] Start HP={hp}/{maxHP}");
    }

    void Update()
    {
        if (dead) return;

        Movement();
        Flip();

        if (Input.GetKeyDown(KeyCode.C))
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
            SoundManager.Instance?.PlayAttack();
        }

        animator.SetBool("IsGrounded", isGround);
        animator.SetFloat("SpeedY", rb ? rb.linearVelocity.y : 0f);

        if (killByFallY && transform.position.y < killY)
        {
            Debug.Log($"[Player] Fell below killY ({killY}) -> DIE");
            ForceDie();
        }
    }

    // ========= HEAL / RESET =========
    public void FullHeal()
    {
        hp = maxHP;
        dead = false;
        invuln = false;
        if (rb) { rb.simulated = true; rb.linearVelocity = Vector2.zero; }
        foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = true;

        // reset các cờ animator có thể làm lệch UI
        animator.ResetTrigger("Hurt");
        animator.ResetTrigger("Attack");
        animator.SetBool("Die", false);
    }

    // ========= MOVE =========
    private void Movement()
    {
        movement = Input.GetAxisRaw("Horizontal");

        animator.SetBool("IsRunning", movement != 0);
        animator.SetBool("IsJumping", !isGround);

        transform.position += new Vector3(movement, 0f, 0f) * Time.deltaTime * moveSpeed;

        if (slope)
        {
            slope.SetMoveInput(movement);
            isGround = slope.Grounded;

            if (Input.GetKeyDown(KeyCode.Space) && isGround)
            {
                slope.RequestJump();
                SoundManager.Instance?.PlayJump();
            }
        }
    }

    private void Flip()
    {
        if (movement > 0 && !facingRight)
        { facingRight = true; var s = transform.localScale; s.x = Mathf.Abs(s.x); transform.localScale = s; }
        else if (movement < 0 && facingRight)
        { facingRight = false; var s = transform.localScale; s.x = -Mathf.Abs(s.x); transform.localScale = s; }
    }

    // ========= ATTACK =========
    public void AttackWindow_On()
    {
        attackWindow = true;
        hitThisSwing.Clear();
        DoAttack();
        attackWindow = false;
    }
    public void AttackWindow_Off() { attackWindow = false; }

    public void DoAttack()
    {
        if (!attackWindow || !attackPoint) return;

        var hits = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 0f, enemyMask);
        foreach (var col in hits)
        {
            var root = col.attachedRigidbody ? col.attachedRigidbody.gameObject : col.gameObject;
            int id = root.GetInstanceID();
            if (hitThisSwing.Contains(id)) continue;

            float dx = root.transform.position.x - transform.position.x;
            if (Mathf.Sign(dx) != (facingRight ? 1f : -1f)) continue;

            var dmgable = root.GetComponentInParent<IDamageable>();
            if (dmgable != null)
            {
                dmgable.TakeDamage(attackDamage);
                hitThisSwing.Add(id);
            }
        }
    }

    // ========= DAMAGE / DEATH =========
    public void TakeDamage(int dmg)
    {
        if (dead || invuln)
        {
            Debug.Log($"[Player] TakeDamage blocked (dead/iframes). dmg={dmg}");
            return;
        }

        int before = hp;
        hp = Mathf.Max(0, hp - dmg);
        Debug.Log($"[Player] Took {dmg} dmg: {before}->{hp}");

        animator.SetTrigger("Hurt");
        StartCoroutine(IFrames());

        if (hp <= 0) Die();
    }

    public void ForceDie()
    {
        if (dead) return;
        hp = 0;
        Die();
    }

    private void Die()
    {
        if (dead) return;
        dead = true;

        // bật anim die trước
        if (HasParam(animator, dieTrigger, AnimatorControllerParameterType.Trigger))
            animator.SetTrigger(dieTrigger);
        else
            animator.CrossFade(dieState, 0.03f, 0, 0f); // fallback

        if (rb) { rb.linearVelocity = Vector2.zero; rb.simulated = false; }
        foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;
    }

    // Animation Event ở cuối clip death
    public void OnDeathAnimFinished()
    {
        if (!gameOverUI) gameOverUI = FindObjectOfType<GameOverUI>(true);
        if (gameOverUI) gameOverUI.Show();   // pause bằng Time.timeScale = 0
    }

    bool HasParam(Animator a, string name, AnimatorControllerParameterType type)
    {
        foreach (var p in a.parameters)
            if (p.name == name && p.type == type) return true;
        return false;
    }

    IEnumerator IFrames()
    {
        invuln = true;
        yield return new WaitForSeconds(invulnTime);
        invuln = false;
        Debug.Log("[Player] i-frames end");
    }

    // ========= DEBUG UI =========
    private void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }

    void OnGUI()
    {
        if (!showSimpleHpText) return;
        GUI.Label(new Rect(10, 10, 220, 22), $"Health: {hp}/{maxHP}");
    }

    // expose
    public int CurrentHP => hp;
    public bool FacingRight => facingRight;
}