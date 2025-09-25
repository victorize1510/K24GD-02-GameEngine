using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class SlopeMove : MonoBehaviour
{
    [Header("Slope settings")]
    public LayerMask groundMask;
    public float checkDist = 0.12f;   // khoảng kiểm tra sát chân
    public float snapForce = 50f;     // lực bám dốc (tăng nếu còn trôi)
    public float maxSlopeAngle = 60f; // dốc tối đa vẫn coi là sàn

    public bool Grounded { get; private set; }

    private Rigidbody2D rb;
    private BoxCollider2D box;
    private PlayerController playerCtrl;

    private float moveInput;
    private bool wantJump;
    private Vector2 groundNormal = Vector2.up;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        playerCtrl = GetComponent<PlayerController>();
        rb.freezeRotation = true;
    }

    // --- API cho PlayerController ---
    public void SetMoveInput(float x) => moveInput = Mathf.Clamp(x, -1f, 1f);
    public void RequestJump() => wantJump = true;

    void FixedUpdate()
    {
        GroundCheck();

        // Nhảy (thực thi ở FixedUpdate để ổn định vật lý)
        if (wantJump && Grounded)
        {
            rb.gravityScale = 3f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * playerCtrl.jumpHeight, ForceMode2D.Impulse);
        }
        wantJump = false;

        if (Grounded)
        {
            float angle = Vector2.Angle(groundNormal, Vector2.up);
            bool onSlope = angle > 0.01f && angle <= maxSlopeAngle;

            if (onSlope)
            {
                // Tiếp tuyến mặt dốc
                Vector2 tangent = new Vector2(-groundNormal.y, groundNormal.x).normalized;

                rb.gravityScale = 0f; // khóa trọng lực khi bám dốc

                if (Mathf.Approximately(moveInput, 0f))
                {
                    // Đứng yên: khử sạch vận tốc dọc dốc để không trượt
                    float vtAlong = Vector2.Dot(rb.linearVelocity, tangent);
                    rb.linearVelocity -= vtAlong * tangent;
                    if (rb.linearVelocity.sqrMagnitude < 0.0004f) rb.linearVelocity = Vector2.zero;
                }
                else
                {
                    // Di chuyển trên dốc theo tiếp tuyến
                    Vector2 targetVel = tangent * (moveInput * playerCtrl.moveSpeed);
                    rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVel, 0.25f);
                }

                // Bám xuống mặt dốc, tránh bật ở mép tile
                rb.AddForce(-groundNormal * snapForce, ForceMode2D.Force);
            }
            else
            {
                // Mặt phẳng
                rb.gravityScale = 3f;
                rb.linearVelocity = new Vector2(moveInput * playerCtrl.moveSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            // Trên không
            rb.gravityScale = 3f;
            rb.linearVelocity = new Vector2(moveInput * playerCtrl.moveSpeed, rb.linearVelocity.y);
        }
    }

    void GroundCheck()
    {
        Vector2 center = (Vector2)transform.position + box.offset;
        Vector2 feet = center + Vector2.down * (box.size.y * 0.5f - 0.01f);
        RaycastHit2D hit = Physics2D.BoxCast(
            feet,
            new Vector2(box.size.x * 0.9f, 0.05f),
            0f,
            Vector2.down,
            checkDist,
            groundMask
        );

        Grounded = hit.collider != null;
        groundNormal = Grounded ? hit.normal : Vector2.up;
    }
}