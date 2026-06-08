using UnityEngine;

public class CombatPauseMenu : MonoBehaviour
{
    public static CombatPauseMenu Instance;

    public GameObject panel;

    bool paused;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Toggle()
    {
        paused = !paused;

        panel.SetActive(paused);

        Time.timeScale = paused ? 0 : 1;
    }

    public void Resume()
    {
        paused = false;

        panel.SetActive(false);

        Time.timeScale = 1;
    }

    public void RetryBattle()
    {
        Time.timeScale = 1;

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void ExitBattle()
    {
        Time.timeScale = 1;

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}