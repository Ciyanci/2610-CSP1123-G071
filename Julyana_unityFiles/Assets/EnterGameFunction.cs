using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterGameFunction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void EnterGameClick()
    {
        Debug.Log("Enter Game Button Clicked");
        SceneManager.LoadScene("Game");
    }
}

    // Update is called once per frame
  