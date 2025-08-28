using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private float DistancesDestroy = 10;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DestroyIFTrue();
    }
    void DestroyIFTrue()
    {
        Vector3 CenterScreen = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Debug.Log(Vector3.Distance(CenterScreen, transform.position));
        Debug.Log(transform.position);
        if (Vector3.Distance(CenterScreen, transform.position) > DistancesDestroy)
        {
            Destroy(this.gameObject);
        }
    }
}
