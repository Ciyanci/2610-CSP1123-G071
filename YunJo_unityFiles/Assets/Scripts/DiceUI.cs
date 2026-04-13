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

    public void Follow(Transform t)
    {
        follow = t;
    }

    void Update()
    {
        if (!follow) return;

        Vector3 screen = Camera.main.WorldToScreenPoint(follow.position + Vector3.up * 1.2f);
        RectTransform rect = transform as RectTransform;
        rect.position = screen;
    }

    public IEnumerator Roll(int min, int max, System.Action<int> onDone)
    {
        canvasGroup.alpha = 1;
        gameObject.SetActive(true);
        text.gameObject.SetActive(true);

        float t = 0;
        float dur = 0.4f;

        int final = Random.Range(min, max + 1);
    
        while (t < dur)
        {
            text.text = Random.Range(min, max + 1).ToString();
            t += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        text.text = final.ToString();
        yield return new WaitForSeconds(0.3f);
        onDone?.Invoke(final);
    }
    public void SetResultColor(int a,int b)
    {
        if (a > b)
        {
            text.color = winColor;
        }
        else if (b > a)
        {
            text.color = loseColor;
        }
        else
        {
            text.color = tieColor;
        }
    }
}