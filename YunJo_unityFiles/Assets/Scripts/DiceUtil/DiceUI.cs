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

    // -------------------------
    // FOLLOW TARGET
    // -------------------------
    public void Follow(Transform t)
    {
        follow = t;
    }

    void LateUpdate()
    {
        if (!follow || !cam) return;

        Vector3 worldPos = follow.position + Vector3.up * 0.5f;
        Vector3 screen = cam.WorldToScreenPoint(worldPos);

        if (screen.z <= 0) return;

        smoothedScreen = Vector3.Lerp(smoothedScreen, screen, Time.deltaTime * 15f);
        rect.position = smoothedScreen;
    }

    // -------------------------
    // VISIBILITY
    // -------------------------
    public void Show()
    {
        canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
    }

    // -------------------------
    // ROLL
    // -------------------------
    public IEnumerator Roll(int min, int max, System.Action<int> onDone)
    {
        Show();
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

        onDone?.Invoke(final);
    }

    // -------------------------
    // SET FINAL COLOR
    // -------------------------
    public void SetResult(int myRoll, int enemyRoll)
    {
        if (myRoll == enemyRoll) text.color = tieColor;
        else if (myRoll > enemyRoll) text.color = winColor;
        else text.color = loseColor;
    }
}