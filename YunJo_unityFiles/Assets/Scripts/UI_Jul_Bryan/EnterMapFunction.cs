using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterMapFunction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void EnterMapClick()
    {
        Debug.Log("Map Button Clicked");
        SceneManager.LoadScene("MapSystem");
    }
}

    // Update is called once per frame
  