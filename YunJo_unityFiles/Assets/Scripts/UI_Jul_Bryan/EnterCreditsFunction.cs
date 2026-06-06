using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterCreditsFunction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void EnterCreditsClick()
    {
        Debug.Log("Credits Button Clicked");
        SceneManager.LoadScene("CreditsScene");
    }
}

    // Update is called once per frame
  