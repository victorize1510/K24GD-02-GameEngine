using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
public class RedChicken : MonoBehaviour
{
    public GameObject EggPrefabs;
    public GameObject FogPrefab;   // gán prefab Fog vào đây trong Inspector
    private Animator animator;

    private bool inFormation = false;
    private Vector3 targetPosition;
    private float moveSpeed = 5f;   // tốc độ bay vào
    private float arriveThreshold = 0.1f; // khoảng cách coi như đã đến
    public int health = 3; // mặc định gà thường cần 3 viên mới chết
    public GameObject giftPrefab;
    public float giftDropChance = 0.3f;
    public int score;
    public GameObject ChickenLegPrefab;

    void Start()
    {
         // Tự động lấy Animator trên chính RedChicken
        animator = GetComponent<Animator>();
        int enemyLayer = LayerMask.NameToLayer("Chicken");
        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
        // Lấy targetPosition chính là vị trí đặt sẵn trong đội hình
        targetPosition = transform.position;

        // Spawn gà ngoài màn hình trước
        float randomX = Random.value > 0f ? -12f : 12f; // trái hoặc phải ngoài màn hình
        transform.position = new Vector3(randomX, targetPosition.y, 0f);

        // Bắt đầu di chuyển vào đội hình
        StartCoroutine(MoveToFormation());
    }

    IEnumerator MoveToFormation()
    {
        while (Vector3.Distance(transform.position, targetPosition) > arriveThreshold)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Khi đã vào đội hình
        inFormation = true;
        StartCoroutine(SpawmEgg());
    }

    IEnumerator SpawmEgg()
    {
        while (true)
        {
            if (inFormation)
            {
                yield return new WaitForSeconds(Random.Range(3f, 20f));
                Instantiate(EggPrefabs, transform.position, Quaternion.identity);
            }
            else
            {
                yield return null;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            health--; // trừ máu khi dính đạn
            Destroy(collision.gameObject);
            if (health <= 0)
            {
                // 1. Tạo fog tại vị trí gà
                GameObject fog = Instantiate(FogPrefab, transform.position, Quaternion.identity);

                // 2. Hủy fog sau khi animation chạy xong
                Destroy(fog, 0.5f); // đổi 0.5f thành đúng thời gian dài của animation Fog

                // 3. Random rơi Gift
                    if (giftPrefab != null && Random.value <= giftDropChance)
                    {
                        Instantiate(giftPrefab, transform.position, Quaternion.identity);
                    }

                // 4. Hủy gà ngay lập tức
                Instantiate(ChickenLegPrefab, transform.position, Quaternion.identity);
                ScoreManager.instance.GetScore(score);
                Destroy(gameObject);
            }
        }
    }
}
