using UnityEngine;

public static class ActionPlanner
{
    public static void AssignToSlot(
        CharacterUnit user,
        SpeedSlot slot,
        Card card,
        CharacterUnit target)
    {
        if (slot == null || card == null || target == null)
            return;

        slot.Assign(card, target, user);

        Debug.Log($"[PLAN] {user.unitName} -> S{slot.value}");
    }
}