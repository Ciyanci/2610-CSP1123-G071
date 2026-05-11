using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CombatStateMachine : MonoBehaviour
{
    public CombatPhase phase;
    public List<EnemyAI> enemies;
    List<CharacterDeck> playerDecks;
    void Awake()
    {
        playerDecks = new List<CharacterDeck>(
            FindObjectsByType<CharacterDeck>(FindObjectsSortMode.None)
        ).FindAll(d => d.owner != null && d.owner.CompareTag("Player"));
    }
    void Start()
    {
        StartCoroutine(TurnLoop());
    }
    IEnumerator TurnLoop()
    {
        while (true)
        {
            yield return StartCoroutine(StartTurn());
            yield return StartCoroutine(DrawPhase());
            yield return StartCoroutine(PlanningPhase());
            yield return StartCoroutine(IntentPreview());
            yield return StartCoroutine(ResolvePhase());
            yield return StartCoroutine(EndTurn());
        }
    }
    // =========================
    // START TURN
    // =========================
    IEnumerator StartTurn()
    {
        phase = CombatPhase.StartTurn;
        Debug.Log("[PHASE] Start Turn");
        // Roll fresh speed dice for everyone — this also clears last turn's slots
        foreach (var unit in UnitRegistry.Instance.players)
            unit.ResetSpeedSlots();
        foreach (var unit in UnitRegistry.Instance.enemies)
            unit.ResetSpeedSlots();
        yield return null;
    }
    // =========================
    // DRAW
    // =========================
    IEnumerator DrawPhase()
    {
        phase = CombatPhase.Draw;
        Debug.Log("[PHASE] Draw");
        // FIX #5: refresh hand to 4 each turn, not top-up to 9
        foreach (var deck in playerDecks)
        {
            if (deck != null)
                deck.RefreshHand();
        }
        // Also refresh enemy hands
        foreach (var enemy in enemies)
        {
            if (enemy != null && enemy.deck != null)
                enemy.deck.RefreshHand();
        }
        HandUI hand = FindFirstObjectByType<HandUI>();
        if (hand != null)
            hand.Hide();
        yield return new WaitForSeconds(0.2f);
    }
    // =========================
    // PLANNING
    // =========================
    IEnumerator PlanningPhase()
    {
        phase = CombatPhase.Planning;
        Debug.Log("[PHASE] Planning");
        // FIX #4: enemies plan fully BEFORE player input opens.
        // This mirrors LoR — enemy intent is locked in, then revealed.
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                yield return StartCoroutine(enemy.TakeTurn());
        }
        // Now open player input
        CombatFlowController.Instance.SetInputEnabled(true);
        yield return new WaitUntil(() =>
            !CombatFlowController.Instance.inputEnabled
        );
    }
    // =========================
    // INTENT PREVIEW
    // =========================
    IEnumerator IntentPreview()
    {
        phase = CombatPhase.IntentPreview;
        Debug.Log("[PHASE] Intent Preview");
        // Good place to show enemy intent indicators on their slots
        yield return new WaitForSeconds(1.0f);
    }
    // =========================
    // RESOLVE
    // =========================
    IEnumerator ResolvePhase()
    {
        phase = CombatPhase.Resolve;
        Debug.Log("[PHASE] Resolve");
        // FIX #1: commit ALL planned slots before the pipeline reads them.
        // BuildIntents() filters for SlotState.Committed — nothing fires without this.
        foreach (var unit in UnitRegistry.Instance.players)
            unit.CommitAllSlots();
        foreach (var unit in UnitRegistry.Instance.enemies)
            unit.CommitAllSlots();
        yield return CombatPipeline.Instance.ResolveTurn();
    }
    // =========================
    // END TURN
    // =========================
    IEnumerator EndTurn()
    {
        phase = CombatPhase.EndTurn;
        Debug.Log("[PHASE] End Turn");
        // Deck hand refill now lives in DrawPhase via RefreshHand().
        // EndTurn is kept for future effects (status ticks, light refresh, etc.)
        yield return new WaitForSeconds(0.5f);
    }
}