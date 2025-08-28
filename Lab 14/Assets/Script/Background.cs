
using UnityEngine;

public class Background : MonoBehaviour
{
    public float scrollSpeed = 0.2f; //tốc độ cuộn
    private Renderer rend;
    private Vector2 offset;
    void Start()
    {
        rend = GetComponent<Renderer>();
        offset = new Vector2(0, scrollSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        rend.material.mainTextureOffset += offset * Time.deltaTime;
    }
}
