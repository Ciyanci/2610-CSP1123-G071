using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ToggleSettingsButton : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject pauseMenuPanel;
    private bool isPaused = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
        (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        isPaused = !isPaused;
        pauseMenuPanel.SetActive(isPaused);
        settingsPanel.SetActive(false);

        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale=1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSetings()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        isPaused = false;
        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void QuitToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TittleScreen"); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
