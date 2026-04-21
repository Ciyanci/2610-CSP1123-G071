using UnityEngine;
using TMPro;
using System.Collections;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance;

    public CanvasGroup fade;
    public TMP_Text turnText;

    public enum Phase
    {
        Start,
        Draw,
        Planning,
        Clash,
        Resolve,
        End
    }

    public Phase currentPhase;

    int turn = 1;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(RunTurn());
    }

    public IEnumerator RunTurn()
    {
        yield return SetPhase(Phase.Start);
        yield return ShowTurn();

        yield return SetPhase(Phase.Draw);
        yield return new WaitForSeconds(0.2f);

        yield return SetPhase(Phase.Planning);

        yield return new WaitUntil(() =>
            CombatFlowController.Instance.inputEnabled == false);

        yield return SetPhase(Phase.Clash);
        yield return SetPhase(Phase.Resolve);
        yield return SetPhase(Phase.End);

        turn++;
    }

        public IEnumerator SetPhase(Phase p)
        {
            currentPhase = p;
            yield return null;
        }

    public IEnumerator ShowTurn()
    {
        yield return Fade(1);

        turnText.text = "Turn " + turn;

        yield return new WaitForSeconds(1f);

        yield return Fade(0);
    }

    public void HideUI()
    {
        StartCoroutine(Fade(0));
        turnText.text = "";
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