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

    int turn = 0;
    bool running;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(TurnLoop());
    }

    IEnumerator TurnLoop()
    {
        while (true)
        {
            yield return RunTurn();
        }
    }

    public IEnumerator RunTurn()
    {
        if (running) yield break;
        running = true;

        turn++;

        // =========================
        // START PHASE + TURN UI
        // =========================
        yield return SetPhase(Phase.Start);

        Debug.Log($"[TURN] Start Turn {turn}");

        turnText.text = "Turn " + turn;
        yield return Fade(1);

        foreach (var unit in FindObjectsByType<CharacterUnit>(FindObjectsSortMode.None))
        {
            unit.RollSpeed();
        }

        yield return new WaitForSeconds(0.4f);

        yield return Fade(0);
        turnText.text = "";

        // =========================
        // DRAW
        // =========================
        yield return SetPhase(Phase.Draw);
        yield return new WaitForSeconds(0.2f);

        // =========================
        // PLANNING
        // =========================
        yield return SetPhase(Phase.Planning);

        CombatFlowController.Instance.SetInputEnabled(true);

        yield return new WaitUntil(() =>
            CombatFlowController.Instance.inputEnabled == false);

            Debug.Log("[TURN] Planning finished → resolving");

        // =========================
        // CLASH / RESOLVE
        // =========================
        yield return SetPhase(Phase.Clash);

        yield return SetPhase(Phase.Resolve);
        Debug.Log("[COMBAT] Starting resolution");

        yield return SetPhase(Phase.End);
        Debug.Log($"[TURN] End Turn → Next Turn {turn + 1}");

        foreach (var unit in FindObjectsByType<CharacterUnit>(FindObjectsSortMode.None))
        {
            if (unit.deck != null)
                unit.deck.FillHandToLimit();
        }

        running = false;
    }

    public IEnumerator SetPhase(Phase p)
    {
        currentPhase = p;
        yield return null;
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