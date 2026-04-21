using UnityEngine;
using TMPro;
using System.Collections;

public class DiceUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CanvasGroup canvasGroup;

    public Color winColor = Color.green;
    public Color loseColor = Color.red;
    public Color tieColor = Color.yellow;

    Transform follow;
    Camera cam;
    RectTransform rect;

    Vector3 smoothedScreen;

    void Awake()
    {
        cam = Camera.main;
        rect = transform as RectTransform;
        canvasGroup.alpha = 0;
        text.color = Color.white;
    }

    //follows target (headanchor or offsetted thing)
    public void Follow(Transform t)
    {
        follow = t;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        follow = null;
    }

    void LateUpdate()
    {
        if (!follow || !cam) return;

        Vector3 worldPos;

        worldPos = follow.position;

        worldPos += Vector3.up * 0.5f;

        Vector3 screen = cam.WorldToScreenPoint(worldPos);

        if (screen.z <= 0) return;

        smoothedScreen = Vector3.Lerp(smoothedScreen, screen, Time.deltaTime * 15f);

        rect.position = smoothedScreen;
    }

    public IEnumerator Roll(int min, int max, System.Action<int> onDone)
    {
        canvasGroup.alpha = 1;
        text.color = Color.white;

        int final = Random.Range(min, max + 1);

        float t = 0;
        float dur = 0.45f;

        while (t < dur)
        {
            text.text = Random.Range(min, max + 1).ToString();
            t += Time.deltaTime;
            yield return null;
        }

        text.text = final.ToString();
        yield return new WaitForSeconds(0.15f);

        onDone?.Invoke(final);
    }

    //i dont know why this works
    public void SetResultColor(int a, int b)
    {
        if (a == b) text.color = tieColor;
        else if (a < b) text.color = winColor;
        else text.color = loseColor;
    }
}