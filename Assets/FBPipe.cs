using UnityEngine;

public class FBPipe : MonoBehaviour
{
    float moveSpeed = 2;    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {        
        transform.position -= new Vector3(moveSpeed*Time.deltaTime,0,0);

        if (transform.position.x < -7 ) Destroy(gameObject);
    }
}
