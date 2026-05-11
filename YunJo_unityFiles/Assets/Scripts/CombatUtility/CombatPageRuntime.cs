using UnityEngine;
using System.Collections.Generic;

public class CombatPageRuntime
{
    public CharacterUnit owner;
    public CharacterUnit target;
    public Card card;

    public List<PageDie> dice = new();

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
            dice.Add(new PageDie
            {
                data = d,   // ✔ direct assignment
                owner = null
            });
        }
    }

    public PageDie GetCurrentDie()
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