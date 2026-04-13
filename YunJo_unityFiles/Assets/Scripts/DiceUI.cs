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

    Vector3 lockedWorldPos;
    bool isRolling;

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

        if (!isRolling)
        {
            lockedWorldPos = follow.position + Vector3.up * 2.0f;
        }

        Vector3 screen = cam.WorldToScreenPoint(lockedWorldPos);

        if (screen.z <= 0) return;

        rect.position = screen;
    }

    public IEnumerator DiceTossEffect()
    {
        Vector3 start = rect.position;
        Vector3 peak = start + new Vector3(Random.Range(-30f, 30f), 80f, 0);

        float t = 0;
        float dur = 0.25f;

        while (t < dur)
        {
            float e = t / dur;
            rect.position = Vector3.Lerp(start, peak, e);

            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator Roll(int min, int max, System.Action<int> onDone)
    {
        canvasGroup.alpha = 1;
        gameObject.SetActive(true);

        isRolling = true;

        int final = Random.Range(min, max + 1);
        int displayed = min;

        float t = 0;
        float dur = 0.5f;

        StartCoroutine(DiceTossEffect());

        while (t < dur)
        {
            displayed = Random.Range(min, max + 1);
            text.text = displayed.ToString();

            t += Time.deltaTime;
            yield return null;
        }

        text.text = final.ToString();

        yield return new WaitForSeconds(0.5f);

        isRolling = false;
        onDone?.Invoke(final);
    }

    public void SetResultColor(int a, int b)
    {
        // PLEASE WORK BRO HOLY CRAP WHY IS THIS NOT WORKING
        if (a == b)
        {
            text.color = tieColor;
        }
        else if (a > b)
        {
            text.color = winColor;
        }
        else
        {
            text.color = loseColor;
        }
    }
}