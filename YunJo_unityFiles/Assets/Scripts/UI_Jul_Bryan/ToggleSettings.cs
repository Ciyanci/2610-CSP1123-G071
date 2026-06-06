using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    private bool isOpen = false;

    void Update()
    {
        // Escape key or controller Start/Menu button
        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
        (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame))
        {
            ToggleSettings();
        }
    }

    public void ToggleSettings()
    {
        isOpen = !isOpen;
        settingsPanel.SetActive(isOpen);

        // Pause game when settings open
        Time.timeScale = isOpen ? 0f : 1f;
    }

    public void CloseSettings()
    {
        isOpen = false;
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
