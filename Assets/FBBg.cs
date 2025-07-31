using UnityEngine;

public class FBBg : MonoBehaviour
{
    float moveSpeed = 3;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(moveSpeed * Time.deltaTime, 0, 0);

        if (transform.position.x < -7.5) transform.position = new Vector3(8, 0, 0);
    }
}
