using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Spin : MonoBehaviour
{
    float spinDura = 3f;
    public GameObject GOpanel;
    GameObject Gold;
    GameObject BladeShield;
    GameObject Diamond;
    GameObject Egg;
    private void Update()
    {
        Gold = GOpanel.transform.Find("Gold").gameObject;
        BladeShield = GOpanel.transform.Find("BladeShield").gameObject;
        Diamond = GOpanel.transform.Find("Diamond").gameObject;
        Egg = GOpanel.transform.Find("Egg").gameObject;

        if (Time.timeScale == 0)
        {
            if (Input.anyKey) Time.timeScale = 1;
        }
        else
        {
            GOpanel.SetActive(false);
            if (Gold.activeSelf) Gold.SetActive(false);
            if (BladeShield.activeSelf) BladeShield.SetActive(false);
            if (Diamond.activeSelf) Diamond.SetActive(false);
            if (Egg.activeSelf) Egg.SetActive(false);

        }
    }
    private IEnumerator SpinWheel()
    {
        float spinTime = 0f;
        float[] angle = { 90, 180, 270, 360 };
        while (spinTime < spinDura)
        {
            transform.Rotate(0, 0, angle[Random.Range(0,3)] * 0.1f);
            spinTime += Time.deltaTime;
            yield return null;
        }        
        Invoke("GO", 2f);
        
    }
    public void SpinButton()
    {
        StartCoroutine(SpinWheel());
    }

    void GO()
    {
        Time.timeScale = 0;        

        if (Time.timeScale == 0)
        {
            float finalZ = transform.eulerAngles.z;
            
            GOpanel.SetActive(true);

            if ((finalZ > -45f && finalZ < 45f) || finalZ > 315f || finalZ < -315f)
                Gold.SetActive(true);
            else if ((finalZ < -45f && finalZ > -135f) || (finalZ > 225f && finalZ < 315f))
                BladeShield.SetActive(true);

            else if ((finalZ < -135f && finalZ > -225f) || (finalZ > 135f && finalZ < 225f))
                Diamond.SetActive(true);

            else if ((finalZ < -225f && finalZ > -315f) || (finalZ > 45f && finalZ < 135f))
                Egg.SetActive(true);
            
        }
        
    }
}
