using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatStateMachine : MonoBehaviour
{
    public CombatPhase phase;
    public List<EnemyAI> enemies;

    public BattleFlowController flow;
    public TurnSystem turnUI;
    public CardInputHandler input;

    List<CardDeck> playerDecks;
    List<CardDeck> enemyDecks;

    void Awake()
    {
        playerDecks = new List<CardDeck>(FindObjectsByType<CardDeck>(FindObjectsSortMode.None))
            .FindAll(d => d.owner != null && d.owner.CompareTag("Player"));

        enemyDecks = new List<CardDeck>(FindObjectsByType<CardDeck>(FindObjectsSortMode.None))
            .FindAll(d => d.owner != null && d.owner.CompareTag("Enemy"));
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

        yield return turnUI.ShowTurn();
    }

    IEnumerator DrawPhase()
    {
        phase = CombatPhase.Draw;

        for (int i = 0; i < 5; i++)
        {
            foreach (var deck in playerDecks)
                deck.Draw();

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator PlanningPhase()
    {
        phase = CombatPhase.Planning;

        input.SetInputEnabled(true);

        flow.ResetPlanning();

        foreach (var enemy in enemies)
            StartCoroutine(enemy.TakeTurn());

        while (!flow.HasPlayerFinishedPlanning())
            yield return null;

        input.SetInputEnabled(false);
    }

    IEnumerator IntentPreview()
    {
        phase = CombatPhase.IntentPreview;

        flow.BuildAllActions();
        FindFirstObjectByType<TargetingSystem>()
            .DebugDrawIntents();

        yield return new WaitForSeconds(1.5f);
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