using UnityEngine;

[System.Serializable]
public class CombatIntent
{
    public CharacterUnit user;
    public CharacterUnit target;
    public SpeedSlot speedSlot;
    public Card card;
    public int priority;

    public bool IsValid =>
        user != null &&
        target != null &&
        card != null &&
        !user.IsDead &&
        !target.IsDead;

    public CombatPageRuntime CreatePage()
    {
        return new CombatPageRuntime(user, target, card);
    }
}