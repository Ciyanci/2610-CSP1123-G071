using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatStateMachine : MonoBehaviour
{
    public CombatPhase phase;
    public List<EnemyAI> enemies;

    public BattleFlowController flow;
    public TurnSystem turnUI;
    public TurnSystem turnSystem;
    public CardInputHandler input;

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
        yield return null;
    }

    IEnumerator DrawPhase()
    {
        phase = CombatPhase.Draw;

        foreach (var deck in playerDecks)
            deck.OpenDeck();

        yield return new WaitForSeconds(0.2f);
    }

    IEnumerator PlanningPhase()
    {
        phase = CombatPhase.Planning;

        CombatFlowController.Instance.SetInputEnabled(true);

        foreach (var enemy in enemies)
            StartCoroutine(enemy.TakeTurn());

        yield return new WaitUntil(() => !CombatFlowController.Instance.inputEnabled);
    }

    IEnumerator IntentPreview()
    {
        phase = CombatPhase.IntentPreview;

        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator ResolvePhase()
    {
        phase = CombatPhase.Resolve;

        yield return flow.ResolveAll();
    }

    IEnumerator EndTurn()
    {
        phase = CombatPhase.EndTurn;
        yield return new WaitForSeconds(0.5f);
    }
}