using UnityEngine;

public class FBGaMa : MonoBehaviour
{    
    public GameObject pipePrefabs;        
    GameObject ps;

    void Start()
    {        
        InvokeRepeating("SpawnPipe", 0, 3);
    }
    
    void Update()
    {
        
    }

    void SpawnPipe()
    {        
        ps = Instantiate(pipePrefabs, new Vector3(3,Random.Range(-1.4f,1.4f)), Quaternion.identity);        
    }

}
