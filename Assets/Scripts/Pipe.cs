using UnityEngine;

public class Pipe : MonoBehaviour

{
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (transform.position.x <= -3.5f)
        {
            Destroy(gameObject);
        }
    }
    private void Move()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }
}
