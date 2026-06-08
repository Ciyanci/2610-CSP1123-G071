using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class TurnTransitionUI : MonoBehaviour
{
    public static TurnTransitionUI Instance;
    public CanvasGroup blackScreen;
    public float fadeSpeed = 2.5f;
    void Awake()
    {
        Instance = this;
        if (blackScreen != null) blackScreen.alpha = 0f;
    }
    public IEnumerator FadeToBlack()
    {
        while (blackScreen.alpha < 1f)
        {
            blackScreen.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        blackScreen.alpha = 1f;
    }
    public IEnumerator FadeFromBlack()
    {
        while (blackScreen.alpha > 0f)
        {
            blackScreen.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        blackScreen.alpha = 0f;
    }
}