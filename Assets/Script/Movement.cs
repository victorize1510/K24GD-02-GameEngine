//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{    
    public float moveDis = 1f;

    Vector2 targerPos;

    Vector2 tempVec = Vector2.right;

    Coroutine running;


    public GameObject squarePrefab; // Prefab cho ô vuông mới        

    Vector3 tempVec3 = Vector3.left;

    private List<Vector2> positions; // Danh sách lưu trữ các vị trí    

    private List<GameObject> trailingSquares = new List<GameObject>();

    public float speed = 0f;

    public GameObject foodPrefab;

    public GameObject GOPanel;


    void Start()
    {             
        targerPos = transform.position;       
        
        running = StartCoroutine(RunEverySecond());
       
        SpawnFood();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && tempVec != Vector2.down) { tempVec = Vector2.up; tempVec3 = Vector3.up; }
        if (Input.GetKeyDown(KeyCode.A) && tempVec != Vector2.right) { tempVec = Vector2.left; tempVec3 = Vector3.left; }
        if (Input.GetKeyDown(KeyCode.S) && tempVec != Vector2.up) { tempVec = Vector2.down; tempVec3 = Vector3.down; }
        if (Input.GetKeyDown(KeyCode.D) && tempVec != Vector2.left) { tempVec = Vector2.right; tempVec3 = Vector3.right; }
        

        if (tempVec == Vector2.up) transform.rotation = Quaternion.identity;
        if (tempVec == Vector2.down) transform.rotation = Quaternion.Euler(0, 0, 180);
        if (tempVec == Vector2.left) transform.rotation = Quaternion.Euler(0, 0, 90);
        if (tempVec == Vector2.right) transform.rotation = Quaternion.Euler(0, 0, -90);




        if(Time.timeScale == 0)
        {
            GOPanel.SetActive(true);
        }

    }
    IEnumerator RunEverySecond()
    {
        while (true)
        {
            autoMove();            
            yield return new WaitForSeconds(0.5f);
        }
    }
    

    void autoMove()
    {        
        Vector2 previousPos = transform.position;
        
        targerPos += tempVec * moveDis;

        if (targerPos.x > 2.5f) targerPos.x = -3.5f;
        else if (targerPos.x < -2.5f) targerPos.x = 3.5f;
        else if (targerPos.y > 6.5f) targerPos.y = -3.5f;
        else if (targerPos.y < -3.5f) targerPos.y = 6.5f;

        transform.position = targerPos;

        // Move trailing squares
        for (int i = trailingSquares.Count - 1; i >= 0; i--)
        {
            Vector3 target = (i == 0) ? previousPos : trailingSquares[i - 1].transform.position;
            trailingSquares[i].transform.position = Vector3.Lerp(trailingSquares[i].transform.position, target, speed);
        }
    }
    
    //Va chạm k vật lý 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Food")
        {
            Destroy(other.gameObject);
            CreateTrailingSquare();
            SpawnFood();
        }
        if (other.gameObject.tag == "Player")
        {
            // Dừng game
            Time.timeScale = 0f;

            //// Thoát game
            //Application.Quit();

            //// Nếu đang trong chế độ Editor (Unity Editor), bạn có thể dùng:
            //#if UNITY_EDITOR
            //            UnityEditor.EditorApplication.isPlaying = false;
            //#endif

        }
    }


    //Tăng kích cỡ

    void CreateTrailingSquare()
    {        
        GameObject newSquare = Instantiate(squarePrefab, transform.position - tempVec3, Quaternion.identity);
        trailingSquares.Add(newSquare);
    }

    void SpawnFood()
    {
        float x = Mathf.Round(Random.Range(-3, 2)) + 0.5f;
        float y = Mathf.Round(Random.Range(-3, 5)) + 0.5f;
        
        Vector2 spawnPos = new Vector2(x,y);
        Instantiate(foodPrefab,spawnPos,Quaternion.identity);
    }

    public void Up()
    {
        if (tempVec != Vector2.down) {tempVec = Vector2.up; tempVec3 = Vector3.up;        }
    }
    public void Down()
    {
        if (tempVec != Vector2.up)  {tempVec = Vector2.down; tempVec3 = Vector3.down; }
    }
    public void Left()
    {
        if (tempVec != Vector2.right) {tempVec = Vector2.left; tempVec3 = Vector3.left; }
    }
    public void Right()
    {
        if (tempVec != Vector2.left) {tempVec = Vector2.right; tempVec3 = Vector3.right; }
    }

    public void ReloadScene()
    {
        // Lấy tên của scene hiện tại
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Tải lại scene hiện tại
        SceneManager.LoadScene(currentSceneName);

        Time.timeScale = 1.0f;

        GOPanel.SetActive(false);

    }
    public void ExitScene()
    {
        // Nếu đang trong chế độ Editor (Unity Editor), bạn có thể dùng:
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
        // Thoát game
        Application.Quit();
    }
}
