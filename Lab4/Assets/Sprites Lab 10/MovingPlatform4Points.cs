using UnityEngine;

public class MovingPlatform4Points : MonoBehaviour
{
    public Transform[] points; // mảng 4 điểm
    public float speed = 2f;   // tốc độ di chuyển
    private int currentTargetIndex = 0;

    void Start()
    {
        if (points.Length == 0)
        {
            Debug.LogError("Bạn chưa đặt điểm nào cho platform!");
            return;
        }
        transform.position = points[0].position; // bắt đầu ở điểm 0
    }

    void Update()
    {
        if (points.Length == 0) return;

        // Di chuyển tới điểm hiện tại
        transform.position = Vector3.MoveTowards(transform.position, points[currentTargetIndex].position, speed * Time.deltaTime);

        // Kiểm tra nếu đến gần điểm, chuyển sang điểm kế tiếp
        if (Vector3.Distance(transform.position, points[currentTargetIndex].position) < 0.01f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % points.Length; // vòng lặp 0→1→2→3→0
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform); // player di chuyển theo platform
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // player rời platform
        }
    }
}
