using UnityEngine;

public class Gift : MonoBehaviour
{
    public float fallSpeed = 1f; // tốc độ rơi

    void Update()
    {
        // di chuyển xuống
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);


        // nếu rơi ra khỏi màn hình thì destroy
        if (transform.position.y < Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - 1f)
        {
            Destroy(gameObject);
        }
    }
}
