using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelectorController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnNewGameClick()
    {
        Debug.Log("New Game button clicked");
        SceneManager.LoadScene("NewGame");
    }

    public void OnLoadGameClick()
    {
        Debug.Log("Load Game button clicked");
        SceneManager.LoadScene("Game");
    }

    public void OnSettingsClick()
    {
        Debug.Log("Settings button clicked");
        SceneManager.LoadScene("Settings");
    }

    public void OnLibraryClick()
    {
        Debug.Log("Library button clicked");
        SceneManager.LoadScene("Library");
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