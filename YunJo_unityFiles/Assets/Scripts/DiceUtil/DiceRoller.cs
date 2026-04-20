using UnityEngine;
using System.Collections;
using TMPro;

public class DiceRoller : MonoBehaviour
{
    public TMP_Text text;
    public CanvasGroup canvasGroup;

    public IEnumerator Roll(int min, int max, Transform follow, System.Action<int> result)
    {
        canvasGroup.alpha = 1;

        int final = Random.Range(min, max + 1);
        float t = 0;
        float dur = 0.5f;

        while (t < dur)
        {
            text.text = Random.Range(min, max + 1).ToString();
            t += Time.deltaTime;
            yield return null;
        }

        text.text = final.ToString();
        yield return new WaitForSeconds(0.25f);

        canvasGroup.alpha = 0;
        result?.Invoke(final);
    }
}