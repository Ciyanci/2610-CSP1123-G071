using UnityEngine;

[System.Serializable]
public class CombatIntent
{
    public CharacterUnit user;
    public CharacterUnit target;

    public SpeedDie speedDie;
    public Card card;

    public bool resolved;

    // resolved at build time
    public int priority;

    // clash result flag (fixes your missing error)
    public bool isClashWinner;

    public CombatPageRuntime CreatePage()
    {
        return new CombatPageRuntime(user, target, card);
    }
}