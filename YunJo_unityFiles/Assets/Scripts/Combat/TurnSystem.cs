using UnityEngine;
using TMPro;
using System.Collections;

public class TurnSystem : MonoBehaviour
{
    public CanvasGroup fade;
    public TMP_Text turnText;

    int turn = 1;

    public IEnumerator NextTurn()
    {
        yield return Fade(1);

        turnText.text = "Turn " + turn;
        turn++;

        yield return new WaitForSeconds(1f);

        yield return Fade(0);
    }

    IEnumerator Fade(float target)
    {
        float t = 0;
        float dur = 0.5f;

        float start = fade.alpha;

        while (t < dur)
        {
            fade.alpha = Mathf.Lerp(start, target, t / dur);
            t += Time.deltaTime;
            yield return null;
        }

        fade.alpha = target;
    }
}