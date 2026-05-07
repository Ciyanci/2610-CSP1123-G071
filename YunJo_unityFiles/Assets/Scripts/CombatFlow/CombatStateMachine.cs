using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatStateMachine : MonoBehaviour
{
    public CombatPhase phase;
    public List<EnemyAI> enemies;

    public BattleFlowController flow;

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
        yield return null;
    }

    IEnumerator DrawPhase()
    {
        phase = CombatPhase.Draw;
        Debug.Log("[PHASE] Draw");

        HandUI hand = FindFirstObjectByType<HandUI>();
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

        // Wait until resolve button disables input
        yield return new WaitUntil(() => !CombatFlowController.Instance.inputEnabled);
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

        yield return flow.ResolveAll();
    }

    IEnumerator EndTurn()
    {
        phase = CombatPhase.EndTurn;
        Debug.Log("[PHASE] End Turn");

        // 🔥 Mahjong discard rule (your requirement)
        foreach (var deck in playerDecks)
        {
            deck.FillHandToLimit(); // ensures empty slots refill
        }

        yield return new WaitForSeconds(0.5f);
    }
}