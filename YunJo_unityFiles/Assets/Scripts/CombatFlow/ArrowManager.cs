using UnityEngine;
using System.Collections.Generic;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager Instance;

    [Header("Prefabs")]
    public CombatArrow previewArrowPrefab;   //blue dashed
    public CombatArrow plannedArrowPrefab;   //red solid
    public CombatArrow clashArrowPrefab;     //yellow solid

    [Header("Tip Sprites — set on prefabs")]

    //cursor following live arrow
    CombatArrow previewArrow;

    //planned arrow (sets for one per slot this time oops)
    Dictionary<SpeedSlot, CombatArrow> plannedArrows = new();

    void Awake()
    {
        Instance = this;

        previewArrow = Instantiate(previewArrowPrefab, transform);
        previewArrow.Hide();
    }

    //preview arrow
    public void UpdatePreview(Vector3 from, Vector3 to)
    {
        previewArrow.Show();
        previewArrow.Draw(from, to);
    }

    public void HidePreview()
    {
        previewArrow.Hide();
    }

    //planned arrow
    public void AddPlannedArrow(SpeedSlot slot)
    {
        if (slot?.owner == null || slot.target == null) return;
        RemovePlannedArrow(slot);
        SpeedSlot counter = FindCounterSlot(slot);
        bool isClash      = counter != null;
        CombatArrow prefab = isClash ? clashArrowPrefab : plannedArrowPrefab;
        CombatArrow arrow  = Instantiate(prefab, transform);
        plannedArrows[slot] = arrow;
        if (counter != null)
        {
            RemovePlannedArrow(counter);
            CombatArrow counterArrow = Instantiate(clashArrowPrefab, transform);
            plannedArrows[counter] = counterArrow;
        }
    }

    public void RemovePlannedArrow(SpeedSlot slot)
    {
        if (plannedArrows.TryGetValue(slot, out var arrow))
        {
            if (arrow != null) Destroy(arrow.gameObject);
            plannedArrows.Remove(slot);
        }
        SpeedSlot counter = FindCounterSlot(slot);
        if (counter != null && plannedArrows.ContainsKey(counter))
        {
            Destroy(plannedArrows[counter].gameObject);
            plannedArrows.Remove(counter);
            // Only respawn if counter still has a valid target
            if (counter.assignedCard != null && counter.target != null)
            {
                CombatArrow downgraded = Instantiate(plannedArrowPrefab, transform);
                plannedArrows[counter] = downgraded;
            }
        }
    }

    public void ClearAllArrows()
    {
        foreach (var kvp in plannedArrows)
            if (kvp.Value != null) Destroy(kvp.Value.gameObject);

        plannedArrows.Clear();
        previewArrow.Hide();
    }

    //update (redraw arrows)
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
