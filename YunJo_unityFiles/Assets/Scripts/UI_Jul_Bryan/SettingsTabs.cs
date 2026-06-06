using UnityEngine;

public class SettingsTabs : MonoBehaviour
{
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject controlsPanel;

    private void Start()
    {
        OpenAudio();
    }

    public void OpenAudio()
    {
        audioPanel.SetActive(true);
        videoPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }
    public void OpenVideo()
    {
        audioPanel.SetActive(false);
        videoPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }
    public void OpenGameplay()
    {
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }
    public void OpenControls()
    {
        audioPanel.SetActive(false);
        videoPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }
}