using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Drawing;

public class ChapterIntroController : MonoBehaviour
{
    public GameObject overlay;
    public TextMeshProUGUI chapterNumberText;
    public TextMeshProUGUI chapterNameText;
    public GameObject bottomBar;
    public GameObject characterLayer;
    public Image backgroundImage1;      // ← new, drag Background1 here
    public Image backgroundImage2;      // ← new, drag Background2 here

    public float typingSpeed = 0.05f;
    public float holdDuration = 2f;
    public float fadeDuration = 0.5f;

    public void Show(string number, string name, System.Action onComplete)
    {
        StartCoroutine(PlayIntro(number, name, onComplete));
    }

    private IEnumerator PlayIntro(string number, string name, System.Action onComplete)
    {
        // hide UI elements
        bottomBar.SetActive(false);
        characterLayer.SetActive(false);
        ClearBackground();

        // reset
        chapterNumberText.text = "";
        chapterNameText.text = "";
        overlay.SetActive(true);

        // fade in
        yield return StartCoroutine(FadeOverlay(0f, 1f));

        // type chapter number
        yield return StartCoroutine(TypeText(chapterNumberText, number));

        yield return new WaitForSeconds(0.3f);

        // type chapter name
        yield return StartCoroutine(TypeText(chapterNameText, name));

        // hold
        yield return new WaitForSeconds(holdDuration);

        ResetBackground();              // ← restore background before fade out starts

        // fade out
        yield return StartCoroutine(FadeOverlay(1f, 0f));

        overlay.SetActive(false);

        // show UI elements again
        bottomBar.SetActive(true);
        characterLayer.SetActive(true);

        onComplete?.Invoke();
    }
    private void ClearBackground()
    {
        if (backgroundImage1 != null) backgroundImage1.color = UnityEngine.Color.black;
        if (backgroundImage2 != null) backgroundImage2.color = UnityEngine.Color.black;
    }

    private void ResetBackground()
    {
        if (backgroundImage1 != null) backgroundImage1.color = UnityEngine.Color.white;
        if (backgroundImage2 != null) backgroundImage2.color = UnityEngine.Color.white;
    }

    private IEnumerator TypeText(TextMeshProUGUI textObj, string text)
    {
        textObj.text = "";
        foreach (char c in text)
        {
            textObj.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator FadeOverlay(float from, float to)
    {
        CanvasGroup canvasGroup = overlay.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = overlay.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}