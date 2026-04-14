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
    bool locked;

    void Awake()
    {
        cam = Camera.main;
        rect = transform as RectTransform;
    }

    public void Follow(Transform t)
    {
        follow = t;
    }

    void LateUpdate()
    {
        if (!follow || !cam) return;

        Vector3 worldPos;

        if (follow.TryGetComponent<CharacterUnit>(out var unit))
            worldPos = unit.GetSmoothedHead();
        else
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

        yield return new WaitForSeconds(0.4f);

        onDone?.Invoke(final);
    }

    public void SetResultColor(int a, int b)
    {
        if (a == b) text.color = tieColor;
        else if (a > b) text.color = winColor;
        else text.color = loseColor;
    }
}