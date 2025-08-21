
using Mono.Cecil;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 1f;
    private Vector3 target;
    private Enemy enemyScript; // Tham chiếu script Enemy
    void Start()
    {
        target = pointB.position;
        enemyScript = GetComponent<Enemy>();
    }

    void Update()
    {
        // Chỉ di chuyển khi enemy chưa chết
        if (enemyScript != null && !enemyScript.IsDead)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                target = (target == pointA.position) ? pointB.position : pointA.position;

                // Lật sprite theo hướng di chuyển
                Vector3 scale = transform.localScale;
                scale.x = (target == pointB.position) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }
    }
}
