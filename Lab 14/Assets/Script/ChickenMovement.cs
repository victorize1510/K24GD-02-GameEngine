using UnityEngine;

public class ChickenMovement : MonoBehaviour
{
public float moveSpeed = 1f;       // tốc độ di chuyển (giảm số này xuống cho chậm lại)
    public float moveRange = 2f;       // khoảng dao động qua lại (theo trục X)

    private Vector3 startPos;
    private int direction = 1;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // di chuyển qua lại
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        // đổi hướng khi chạm biên
        if (Mathf.Abs(transform.position.x - startPos.x) > moveRange)
        {
            direction *= -1;
        }
    }
}
