using UnityEngine;

public static class ActionPlanner
{
    public static void AssignToSlot(
        CharacterUnit user,
        SpeedSlot slot,
        Card card,
        CharacterUnit target)
    {
        if (user == null || slot == null || card == null || target == null)
            return;

        if (!user.CanAct)
            return;

        if (target.IsDead)
            return;

        slot.Assign(card, target, user);

        Debug.Log($"[PLAN] {user.unitName} -> S{slot.value}");
    }
}