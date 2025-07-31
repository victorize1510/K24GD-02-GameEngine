using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FBCava : MonoBehaviour
{
    public Text pointText;
    public int currentPoint = 0;

    // Update is called once per frame
    void Update()
    {
        pointText.text = currentPoint.ToString();

        if (Time.timeScale == 0 )
        {
            transform.Find("GO").gameObject.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene("Flappy Bird");
    }
}
