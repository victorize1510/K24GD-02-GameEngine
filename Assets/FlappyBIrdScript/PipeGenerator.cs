using UnityEngine;

public class PipeGenerator : MonoBehaviour
{
    public GameObject pipePrefab;
    private float countdown;
    public float timeDuration;
    public bool enableGenratePipe; //cho phep sinh ra ong khi vua bat dau
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        countdown = 1f;
        enableGenratePipe = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (enableGenratePipe == true)
        {
            countdown -= Time.deltaTime; // moi frame countdown = -1 /fps
            //30 FPS: moi frame countdown giam di 1/30s, mot giay (30 frames) thi countdown giam di 1
            if (countdown <= 0)
            {
                //sinh ra 1 ong
                Instantiate(pipePrefab, new Vector3(5, Random.Range(-1.2f, 2.1f), 0), Quaternion.identity);
                countdown = timeDuration;
            }
        }
        
    }
}
