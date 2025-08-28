using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Egg : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    private Collider2D col;
    void Start()
    {
        col = GetComponent<Collider2D>();
        StartCoroutine(CheckEggPosition());
    }

    IEnumerator CheckEggPosition()
    {
        while (true)
        {
            Vector3 viewPort = Camera.main.WorldToViewportPoint(transform.position);
            if (viewPort.y < 0.05f)
            {
                animator.SetTrigger("Break");
                rb.bodyType = RigidbodyType2D.Static;
                // Tắt Collider để không gây chết Player
                if (col != null) col.enabled = false;
                Destroy(gameObject, 1);
                break;
            }
            yield return null;
        }
    }
}
