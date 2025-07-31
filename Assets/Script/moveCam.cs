using UnityEngine;
using UnityEngine.InputSystem;

public class moveCam : MonoBehaviour
{
    public float speed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = Vector3.zero;

        if (Keyboard.current.wKey.isPressed) move += Vector3.up;
        if (Keyboard.current.sKey.isPressed) move += Vector3.down;
        if (Keyboard.current.aKey.isPressed) move += Vector3.left;
        if (Keyboard.current.dKey.isPressed) move += Vector3.right;

        transform.position += move.normalized * speed * Time.deltaTime;
    }
}
