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

    IEnumerator StartTurn()
    {
        phase = CombatPhase.StartTurn;
        Debug.Log("[PHASE] Start Turn");

        foreach (var unit in UnitRegistry.Instance.players)
            unit.ResetSpeedSlots();

        foreach (var unit in UnitRegistry.Instance.enemies)
            unit.ResetSpeedSlots();

        yield return null;
    }

    IEnumerator DrawPhase()
    {
        phase = CombatPhase.Draw;
        Debug.Log("[PHASE] Draw");

        HandUI hand = FindFirstObjectByType<HandUI>();
        if (hand != null)
            hand.Hide();

        yield return new WaitForSeconds(0.2f);
    }

    IEnumerator PlanningPhase()
    {
        phase = CombatPhase.Planning;
        Debug.Log("[PHASE] Planning");

        CombatFlowController.Instance.SetInputEnabled(true);

        foreach (var enemy in enemies)
            StartCoroutine(enemy.TakeTurn());

        yield return new WaitUntil(() =>
            !CombatFlowController.Instance.inputEnabled
        );
    }

    IEnumerator IntentPreview()
    {
        phase = CombatPhase.IntentPreview;
        Debug.Log("[PHASE] Intent Preview");

        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator ResolvePhase()
    {
        phase = CombatPhase.Resolve;
        Debug.Log("[PHASE] Resolve");

        yield return CombatPipeline.Instance.ResolveTurn();
    }
    IEnumerator EndTurn()
    {
        phase = CombatPhase.EndTurn;
        Debug.Log("[PHASE] End Turn");

        foreach (var deck in playerDecks)
        {
            if (deck != null)
                deck.FillHandToLimit();
        }

        yield return new WaitForSeconds(0.5f);
    }
}