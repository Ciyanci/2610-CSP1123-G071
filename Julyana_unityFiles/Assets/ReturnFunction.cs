using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnFunction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ReturnClick()
    {
        Debug.Log("Return Button Clicked");
        SceneManager.LoadScene("MainMenu");
    }
}

    // Update is called once per frame
  