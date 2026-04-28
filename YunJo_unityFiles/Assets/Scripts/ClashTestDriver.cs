using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClashTestDriver : MonoBehaviour
{
    public List<CombatTestIntent> testIntents;

    public BattleFlowController flow;
    public TurnSystem turnSystem;

    bool running;

    void Start()
    {
        PreviewIntents();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!running)
                StartCoroutine(RunTest());
        }
    }

    void PreviewIntents()
    {
        flow.ClearPreview();

        foreach (var t in testIntents)
        {
            if (t.user == null ||
                t.target == null ||
                t.card == null)
                continue;

            flow.QueuePreview(
                t.user,
                t.target,
                t.card
            );
        }
    }

    IEnumerator RunTest()
    {
        running = true;

        // ✅ stop auto turns
        turnSystem.enabled = false;

        yield return flow.ResolveAll();

        running = false;
    }
}