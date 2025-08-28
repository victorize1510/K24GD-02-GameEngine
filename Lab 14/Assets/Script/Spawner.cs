using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Spawner : MonoBehaviour
{
    public GameObject ChickenPrefab;
    public Transform GridChicken;
     public GameObject BossPrefab;
    public Transform BossSpawnPoint;

    private int currentFormation = 0; // theo dõi đội hình hiện tại
    private List<GameObject> currentChickens = new List<GameObject>();
    private bool bossSpawned = false;

    void Start()
    {
        SpawnFormation(currentFormation);
    }

    void Update()
    {
            if (currentChickens.Count == 0)
        {
            // Kiểm tra boss đã spawn chưa
            if (!bossSpawned)
            {
                currentFormation++;
                if (currentFormation < 3) // còn trong danh sách đội hình
                {
                    SpawnFormation(currentFormation);
                }
                else
                {
                    // Đến lượt boss thì chỉ spawn boss thôi
                    SpawnBoss();
                }
            }
        }
        else
        {
            // kiểm tra còn gà sống không
            currentChickens.RemoveAll(ch => ch == null);
        }
    }

    void SpawnFormation(int formationIndex)
    {
        currentChickens.Clear();

        switch (formationIndex)
        {
            case 0:
                SpawnGrid(4, 8); // đội hình lưới 3x6
                break;
            case 1:
                SpawnVShape(5); // đội hình chữ V
                break;
            case 2:
                SpawnCircle(8); // đội hình tròn
                break;
        }
    }

    void SpawnGrid(int rows, int cols, float spacing = 1.5f)
    {
        float startX = -(cols - 1) * spacing / 2f; ;
        float startY = 4f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 pos = new Vector3(startX + j * spacing, startY - i * spacing, 0);
                GameObject chicken = Instantiate(ChickenPrefab, pos, Quaternion.identity, GridChicken);
                currentChickens.Add(chicken);
            }
        }
    }

    void SpawnVShape(int size)
    {
        float startY = 4f;
        for (int i = 0; i < size; i++)
        {
            Vector3 leftPos = new Vector3(-i, startY - i, 0);
            Vector3 rightPos = new Vector3(i, startY - i, 0);

            GameObject c1 = Instantiate(ChickenPrefab, leftPos, Quaternion.identity, GridChicken);
            GameObject c2 = Instantiate(ChickenPrefab, rightPos, Quaternion.identity, GridChicken);

            currentChickens.Add(c1);
            currentChickens.Add(c2);
        }
    }

    void SpawnCircle(int count, float radius = 3f)
    {
        Vector3 center = new Vector3(0, 2f, 0); // tâm vòng tròn (có thể chỉnh)
        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count; // chia đều theo vòng tròn
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            Vector3 pos = center + new Vector3(x, y, 0);
            GameObject chicken = Instantiate(ChickenPrefab, pos, Quaternion.identity, GridChicken);
            currentChickens.Add(chicken);
        }
    }
     void SpawnBoss()
    {
        bossSpawned = true;
        Vector3 spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.2f, 0)); 
        spawnPos.z = 0;
        Instantiate(BossPrefab, spawnPos, Quaternion.identity);
        Debug.Log("Boss xuất hiện!");
    }
}
