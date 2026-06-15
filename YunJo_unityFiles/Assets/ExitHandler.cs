using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            if (currentScene > 0)
            {
                SceneManager.LoadScene(currentScene - 1);
            }
        }
    }
}