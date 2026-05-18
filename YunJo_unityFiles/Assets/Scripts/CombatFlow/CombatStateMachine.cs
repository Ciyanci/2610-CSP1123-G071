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
    //start turn
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
    //draw phase
    IEnumerator DrawPhase()
    {
        phase = CombatPhase.Draw;
        Debug.Log("[PHASE] Draw");
        foreach (var deck in playerDecks)
        {
            if (deck != null)
                deck.RefreshHand();
        }
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
    //planning phase
    IEnumerator PlanningPhase()
    {
        phase = CombatPhase.Planning;
        Debug.Log("[PHASE] Planning");
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
    //intent preview
    IEnumerator IntentPreview()
    {
        phase = CombatPhase.IntentPreview;
        Debug.Log("[PHASE] Intent Preview");
        yield return new WaitForSeconds(1.0f);
    }
    //resolve phase
    IEnumerator ResolvePhase()
    {
        phase = CombatPhase.Resolve;
        Debug.Log("[PHASE] Resolve");
        //BuildIntents() filters for SlotState.Committed so that nothing fires without this
        foreach (var unit in UnitRegistry.Instance.players)
            unit.CommitAllSlots();
        foreach (var unit in UnitRegistry.Instance.enemies)
            unit.CommitAllSlots();
        yield return CombatPipeline.Instance.ResolveTurn();
    }
    //end turn
    IEnumerator EndTurn()
    {
        phase = CombatPhase.EndTurn;
        Debug.Log("[PHASE] End Turn");
        //Deck hand refill is now in DrawPhase via RefreshHand()
        //end turn only for future effects
        yield return new WaitForSeconds(0.5f);
    }
}