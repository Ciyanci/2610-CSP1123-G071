using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatPipeline : MonoBehaviour
{
    public static CombatPipeline Instance;

    public CombatResolver resolver;

    void Awake()
    {
        Instance = this;
    }

    void CommitAllSlots()
    {
        var allUnits = new List<CharacterUnit>();
        allUnits.AddRange(UnitRegistry.Instance.players);
        allUnits.AddRange(UnitRegistry.Instance.enemies);

        foreach (var unit in allUnits)
        {
            foreach (var slot in unit.speedSlots)
            {
                if (slot.state == SlotState.Planned)
                    slot.Commit();
            }
        }
    }

    public IEnumerator ResolveTurn()
    {
        // 🔥 LOCK PHASE (CRITICAL)
        CommitAllSlots();

        var intents = BuildIntentsFromSlots();
        var clashes = ClashDetector.Build(intents);

        yield return resolver.Resolve(new CombatTurnContext
        {
            intents = intents,
            clashes = clashes
        });

        PreviewManager.Instance.Clear();
    }

    // =========================
    // SLOT-DRIVEN BUILD (FINAL FORM)
    // =========================
    List<CombatIntent> BuildIntentsFromSlots()
    {
        List<CombatIntent> result = new();

        var allUnits = new List<CharacterUnit>();
        allUnits.AddRange(UnitRegistry.Instance.players);
        allUnits.AddRange(UnitRegistry.Instance.enemies);

        foreach (var unit in allUnits)
        {
            foreach (var slot in unit.speedSlots)
            {
                if (slot == null) continue;
                if (slot.assignedCard == null) continue;
                if (slot.target == null) continue;
                if (slot.state != SlotState.Committed) continue;

                // 🔥 ensure state consistency (safe guard)
                slot.state = SlotState.Executed;

                result.Add(new CombatIntent
                {
                    user = unit,
                    target = slot.target,
                    card = slot.assignedCard,
                    speedSlot = slot,
                    priority = slot.value
                });
            }
        }

        result.Sort((a, b) => b.priority.CompareTo(a.priority));
        return result;
    }
}