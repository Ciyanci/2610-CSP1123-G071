using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PressAnyButton : MonoBehaviour
{
    public string nextScene = "MenuSelectionScreen";
    public TMP_Text promptText;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(nextScene);
        }

        // makes text blink
        promptText.alpha = Mathf.PingPong(Time.time, 1f);
    }
}