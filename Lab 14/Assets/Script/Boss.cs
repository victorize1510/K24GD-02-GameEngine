using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


public class Boss : MonoBehaviour
{
    public GameObject eggPrefab;
    public GameObject FogPrefab;
    public int Bosshealth = 50;
    public GameObject giftPrefab;
    public float giftDropChance = 0.3f;
    public int score ;
    private GameOverManager gameOverManager;
    void Start()
    {
        StartCoroutine(Spawnegg());
        StartCoroutine(MoveBossRandomPoint());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator Spawnegg()
    {
        while (true)
        {
            Instantiate(eggPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(0.0f, 1.0f));
        }

    }
    IEnumerator MoveBossRandomPoint()
    {
        Vector3 point = getRandomPoint();
        while (transform.position != point)
        {
            transform.position = Vector3.MoveTowards(transform.position, point, 0.1f);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        StartCoroutine(MoveBossRandomPoint());
    }
    Vector3 getRandomPoint()
    {
        Vector3 posRandom = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0f, 1), Random.Range(0.5f, 1)));
        posRandom.z = 0;
        return posRandom;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Bosshealth--; 
            Destroy(collision.gameObject);



            if (Bosshealth <= 0)
            {
                // 1. Tạo fog tại vị trí boss
                if (FogPrefab != null)
                {
                    GameObject fog = Instantiate(FogPrefab, transform.position, Quaternion.identity);
                    Destroy(fog, 0.5f); // đổi 0.5f thành đúng thời gian animation fog
                }

                // 2. Random rơi Gift
                if (giftPrefab != null && Random.value <= giftDropChance)
                {
                    Instantiate(giftPrefab, transform.position, Quaternion.identity);
                }


                // 3. Hủy boss
                Destroy(gameObject);
                ScoreManager.instance.GetScore(score);
                // 4. Hiện panel win
                if (GameOverManager.instance != null)
                    GameOverManager.instance.WinGame();
            }
        }
    }
}
