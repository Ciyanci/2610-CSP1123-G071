using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 1f;

    public void LoadScene(string MenuScreen)
    {
        StartCoroutine(FadeAndLoad(MenuScreen));
    }

    IEnumerator FadeAndLoad(string MenuScreen)
    {
        fadePanel.gameObject.SetActive(true);  // enable it when fade starts
        float timer = 0f;
        Color c = fadePanel.color;
        c.a = 0f;
        fadePanel.color = c;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            c.a = timer / fadeDuration;
            fadePanel.color = c;
            yield return null;
        }
        SceneManager.LoadScene(MenuScreen);
    }
}