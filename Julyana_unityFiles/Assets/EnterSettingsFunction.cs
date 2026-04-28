using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterSettingsFunction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void EnterSettingsClick()
    {
        Debug.Log("Settings Button Clicked");
        SceneManager.LoadScene("SettingsScreen");
    }
}

    // Update is called once per frame
  