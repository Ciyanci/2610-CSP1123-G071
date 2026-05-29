using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        Debug.Log("Start button clicked");
        SceneManager.LoadScene("MenuSelectionScreen");
    }

    public void OnExitClick()
    {
        Debug.Log("Exit button clicked");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}