using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClashTestDriver : MonoBehaviour
{
    public BattleFlowController flow;

    [System.Serializable]
    public class TestPair
    {
        public CharacterUnit attacker;
        public CharacterUnit target;
        public CardData card;
    }

    [Header("DEFINE INTENTS HERE")]
    public List<TestPair> plannedIntents = new();

    bool isRunning = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isRunning) return;
            StartCoroutine(RunTest());
        }
    }

    IEnumerator RunTest()
    {
        isRunning = true;

        Debug.Log("=== TEST START ===");

        // CLEAR OLD
        flow.ClearPreview();

        // -------------------------
        // BUILD PREVIEW (CLEAR + READABLE)
        // -------------------------
        foreach (var p in plannedIntents)
        {
            if (p.attacker == null || p.target == null || p.card == null)
            {
                Debug.LogWarning("[TEST] Invalid intent");
                continue;
            }

            Card cardInstance = new Card(p.card);

            Debug.Log($"[PLAN] {p.attacker.name} → {p.target.name} ({p.card.Name})");

            flow.QueuePreview(p.attacker, p.target, cardInstance);
        }

        // -------------------------
        // DEBUG: SHOW FINAL PLAN
        // -------------------------
        Debug.Log("=== FINAL PLAN ===");

        foreach (var p in flow.previewIntents)
        {
            Debug.Log($"[PREVIEW] {p.user.name} → {p.target.name}");
        }

        // WAIT so arrows are visible
        yield return new WaitForSeconds(1.0f);

        // -------------------------
        // RESOLVE
        // -------------------------
        yield return StartCoroutine(flow.ResolveAll());

        Debug.Log("=== TEST END ===");

        isRunning = false;
    }
}