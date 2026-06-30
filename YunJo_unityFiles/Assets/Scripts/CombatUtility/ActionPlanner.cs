using UnityEngine;
public static class ActionPlanner
{
    public static bool AssignToSlot(
        CharacterUnit user,
        SpeedSlot slot,
        Card card,
        CharacterUnit target)
    {
        if (!user.CanPay(card.Cost))
        {
            Debug.LogWarning($"[PLANNER] {user.unitName} cannot afford " +
                             $"{card.Name} (cost:{card.Cost} light:{user.currentLight})");
            return false;
        }
        user.SpendLight(card.Cost);
        slot.Plan(card, target);
        return true;
    }
}