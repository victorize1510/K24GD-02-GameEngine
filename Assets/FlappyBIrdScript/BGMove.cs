using UnityEngine;

public class BGMove : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)Vector2.left * 1f * Time.deltaTime;
        if (transform.position.x <= -7.2f)
        {
            transform.position = new Vector3(7f, -0.24f, 0);
        }
    }
}
