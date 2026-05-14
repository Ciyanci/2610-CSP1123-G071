using UnityEngine;
using System.Collections.Generic;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager Instance;

    [Header("Prefabs")]
    public CombatArrow previewArrowPrefab;   // blue dashed
    public CombatArrow plannedArrowPrefab;   // red solid
    public CombatArrow clashArrowPrefab;     // yellow solid

    [Header("Tip Sprites — set on prefabs")]
    // Tips are child Transforms on each prefab, already configured

    // Live preview arrow (cursor-following)
    CombatArrow previewArrow;

    // Planned arrows — one per committed slot
    // Key: the SpeedSlot that owns this arrow
    Dictionary<SpeedSlot, CombatArrow> plannedArrows = new();

    void Awake()
    {
        Instance = this;

        previewArrow = Instantiate(previewArrowPrefab, transform);
        previewArrow.Hide();
    }

    // =========================
    // PREVIEW ARROW
    // Called every frame by CombatFlowController while targeting
    // =========================
    public void UpdatePreview(Vector3 from, Vector3 to)
    {
        previewArrow.Show();
        previewArrow.Draw(from, to);
    }

    public void HidePreview()
    {
        previewArrow.Hide();
    }

    // =========================
    // PLANNED ARROW
    // Call when a slot is assigned
    // =========================
    public void AddPlannedArrow(SpeedSlot slot)
    {
        if (slot?.owner == null || slot.target == null) return;

        // Remove old arrow if re-assigning
        RemovePlannedArrow(slot);

        // Check if counter-slot exists → clash
        SpeedSlot counter = FindCounterSlot(slot);
        bool isClash      = counter != null;

        CombatArrow prefab = isClash ? clashArrowPrefab : plannedArrowPrefab;
        CombatArrow arrow  = Instantiate(prefab, transform);

        plannedArrows[slot] = arrow;

        // If clash, also upgrade the counter's arrow
        if (isClash && plannedArrows.TryGetValue(counter, out var counterArrow))
        {
            RemovePlannedArrow(counter);
            CombatArrow newCounter = Instantiate(clashArrowPrefab, transform);
            plannedArrows[counter] = newCounter;
        }
    }

    public void RemovePlannedArrow(SpeedSlot slot)
    {
        if (plannedArrows.TryGetValue(slot, out var arrow))
        {
            if (arrow != null) Destroy(arrow.gameObject);
            plannedArrows.Remove(slot);
        }
    }

    public void ClearAllArrows()
    {
        foreach (var kvp in plannedArrows)
            if (kvp.Value != null) Destroy(kvp.Value.gameObject);

        plannedArrows.Clear();
        previewArrow.Hide();
    }

    // =========================
    // UPDATE — redraw all arrows each frame
    // Positions update even if units move
    // =========================
    void Update()
    {
        foreach (var kvp in plannedArrows)
        {
            SpeedSlot   slot   = kvp.Key;
            CombatArrow arrow  = kvp.Value;

            if (slot == null || arrow == null) continue;
            if (slot.owner == null || slot.target == null) continue;

            // Clash arrows end at midpoint, not at target
            SpeedSlot counter = FindCounterSlot(slot);
            bool isClash      = counter != null;

            Vector3 from = slot.owner.clashAnchor != null
                ? slot.owner.clashAnchor.position
                : slot.owner.transform.position;

            Vector3 to = slot.target.clashAnchor != null
                ? slot.target.clashAnchor.position
                : slot.target.transform.position;

            if (isClash)
                arrow.DrawToMidpoint(from, to);
            else
                arrow.Draw(from, to);
        }
    }

    SpeedSlot FindCounterSlot(SpeedSlot slot)
    {
        if (slot?.target == null) return null;

        foreach (var s in slot.target.speedSlots)
        {
            if (s.target == slot.owner &&
                (s.state == SlotState.Planned ||
                 s.state == SlotState.Committed))
                return s;
        }
        return null;
    }
}
