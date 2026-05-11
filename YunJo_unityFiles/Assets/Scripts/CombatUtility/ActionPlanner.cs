using UnityEngine;

public static class ActionPlanner
{
    public static void AssignToSlot(
        CharacterUnit owner,
        SpeedSlot slot,
        Card card,
        CharacterUnit target)
    {
        if (owner == null || slot == null || card == null || target == null)
            return;

        if (!owner.CanAct)
            return;

        if (target.IsDead)
            return;

        slot.Plan(card, target);

        Debug.Log($"[PLAN] {owner.unitName} -> S{slot.value}");
        SlotUIUpdater.Refresh(owner);
    }
}