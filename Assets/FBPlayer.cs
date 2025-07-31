using UnityEngine;

public class FBPlayer : MonoBehaviour
{
    float jumpForce = 5f;
    Rigidbody2D rb;
    Animator animator;
    public AudioSource flySound;
    public AudioSource hitSound;
    public AudioSource dieSound;
    public AudioSource pointSound;
    public GameObject canvas;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();        
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Fly();
        }
    }

    void Fly()
    {
        rb.AddForce(new Vector3(0, jumpForce), ForceMode2D.Impulse);
        animator.SetTrigger("fly");
        flySound.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Died");
            
            hitSound?.Play();

            Time.timeScale = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Point"))
        {
            Debug.Log("+ 1");
            pointSound?.Play();
            canvas.GetComponent<FBCava>().currentPoint += 1;
        }
    }
}
