using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatStateMachine : MonoBehaviour
{
    public static CombatStateMachine Instance;
    public CombatPhase phase;

    // ✅ Set true by TutorialManager while it owns input
    // Prevents PlanningPhase from reacting to tutorial's internal enable/disable
    [HideInInspector] public bool tutorialControllingInput = false;

    void Awake() => Instance = this;

    void Start() => StartCoroutine(TurnLoop());

    IEnumerator TurnLoop()
    {
        while (true)
        {
            yield return StartTurnPhase();
            yield return DrawPhase();
            yield return PlanningPhase();
            yield return new WaitUntil(() => _turnComplete);
            _turnComplete = false;
        }
    }

    bool _turnComplete = false;
    public void NotifyTurnComplete() => _turnComplete = true;

    IEnumerator StartTurnPhase()
    {
        phase = CombatPhase.StartTurn;
        Debug.Log("[PHASE] Start Turn");
        // ✅ Always reset tutorial flag at the start of a fresh turn
        // After the tutorial step that uses keepInputAfter, subsequent turns
        // should run normally with no tutorial interference
        tutorialControllingInput = false;
        foreach (var unit in UnitRegistry.Instance.players)
        {
            if (unit == null || unit.IsDead) continue;
            unit.ResetSpeedSlots();
        }
        foreach (var unit in UnitRegistry.Instance.enemies)
        {
            if (unit == null || unit.IsDead) continue;
            unit.ResetSpeedSlots();
        }
        yield return new WaitForSeconds(0.8f);
        CombatHUDController.Instance?.ShowSpeedBubbles();
    }

    IEnumerator DrawPhase()
    {
        phase = CombatPhase.Draw;
        Debug.Log("[PHASE] Draw");

        foreach (var unit in UnitRegistry.Instance.players)
            unit.deck?.RefreshHand();
        foreach (var unit in UnitRegistry.Instance.enemies)
            unit.deck?.RefreshHand();

        yield return new WaitForSeconds(0.3f);

        var players = UnitRegistry.Instance.players;
        if (players.Count > 0 && players[0].deck != null)
            HandUI.Instance?.Show(players[0].deck);

        CombatInfoBar.Instance?.ShowDefault();
    }

    IEnumerator PlanningPhase()
    {
        phase = CombatPhase.Planning;
        Debug.Log("[PHASE] Planning");
        yield return new WaitUntil(() => !tutorialControllingInput);
        // Run enemy AI
        var enemies = UnitRegistry.Instance?.enemies;
        if (enemies != null)
        {
            foreach (var enemy in enemies)
            {
                if (enemy == null || enemy.IsDead) continue;
                var ai = enemy.GetComponent<EnemyAI>();
                if (ai == null) continue;
                Debug.Log($"[AI] Running AI for {enemy.unitName}");
                yield return ai.TakeTurn();
            }
        }
        RefreshInfoBarForEnemyIntent();
        CombatInfoBar.Instance?.ShowDefault();
        yield return new WaitUntil(() => !tutorialControllingInput);
        CombatFlowController.Instance.SetInputEnabled(true);
        yield return new WaitUntil(() =>
            !CombatFlowController.Instance.inputEnabled &&
            !tutorialControllingInput);
    }
    void RefreshInfoBarForEnemyIntent()
    {
        var players = UnitRegistry.Instance?.players;
        if (players == null) return;

        foreach (var player in players)
        {
            if (player == null || player.IsDead) continue;
            foreach (var slot in player.speedSlots)
            {
                if (slot.state == SlotState.Planned &&
                    slot.assignedCard != null &&
                    slot.target != null)
                {
                    CombatInfoBar.Instance?.ShowSlotInfo(slot);
                    return;
                }
            }
        }
    }
}
