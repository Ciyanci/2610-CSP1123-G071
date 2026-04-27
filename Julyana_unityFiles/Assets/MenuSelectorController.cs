using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelectorController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnNewGameClick()
    {
        SceneManager.LoadScene("MapSystem");
    }

    public void OnLoadGameClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnSettingsClick()
    {
        SceneManager.LoadScene("Settings");
    }

    public void OnLibraryClick()
    {
        SceneManager.LoadScene("Library");
    }

    public void OnReturnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

}