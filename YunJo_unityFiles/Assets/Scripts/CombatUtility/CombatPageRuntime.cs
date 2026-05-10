using UnityEngine;
using System.Collections.Generic;

public class CombatPageRuntime
{
    public CharacterUnit owner;
    public CharacterUnit target;

    public Card card;

    public List<CombatDiceRuntime> dice = new();

    public int currentIndex;

    public bool IsFinished =>
        currentIndex >= dice.Count;

    public CombatPageRuntime(
        CharacterUnit owner,
        CharacterUnit target,
        Card card)
    {
        this.owner = owner;
        this.target = target;
        this.card = card;

        foreach (var d in card.GetDice())
        {
            dice.Add(new CombatDiceRuntime(d));
        }
    }

    public CombatDiceRuntime GetCurrentDie()
    {
        if (IsFinished)
            return null;

        return dice[currentIndex];
    }

    public void Advance()
    {
        currentIndex++;
    }
}