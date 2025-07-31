using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    float speed = 5f;
    float jumpForce = 6f;
    Animator animator;
    Vector2 move;
    Rigidbody2D rb;    

    public GameObject bulletPrefab;
    GameObject bullet;
    public GameObject firePos;
    public GameObject target;
    bool isjump = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();       

    }

    // Update is called once per frame
    void Update()
    {        

        move.x = Input.GetAxis("Horizontal");
        

        transform.position += (Vector3)move * speed * Time.deltaTime;

        animator.SetFloat("speed",Mathf.Abs(move.x));

        if (move.x > 0) transform.localScale = new Vector3(1, 1, 1);
        if (move.x < 0) transform.localScale = new Vector3(-1, 1, 1);

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("attack");
        }

        //Jump 

        if (Input.GetKeyDown(KeyCode.Space) && isjump==false)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isjump = true;
            animator.SetTrigger("jump");
        }


        //fire

        if (Input.GetKeyDown(KeyCode.F))
        {
            bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        }
        if (bullet != null)
        {
            bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, target.transform.position, 5f * Time.deltaTime);
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        }
        if (bullet != null)
        {
            bullet.transform.position = Vector2.MoveTowards(bullet.transform.position, target.transform.position, 5f * Time.deltaTime);

            if (Mathf.Abs(bullet.transform.position.x - target.transform.position.x) < 0.1f) { Destroy(bullet); }
        }
        


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isjump = false;
        }
    }
}
