using System.Collections.Generic;

public class CombatPageRuntime
{
    public CharacterUnit owner;
    public CharacterUnit target;   // ← already exists, just confirm it's set
    public Card card;
    public List<PageDie> dice = new();
    public int currentIndex;
    public bool IsFinished => currentIndex >= dice.Count;
    public CombatPageRuntime(CharacterUnit owner, CharacterUnit target, Card card)
    {
        this.owner  = owner;
        this.target = target;   // ← confirm this line is present
        this.card   = card;
        foreach (var d in card.GetDice())
        {
            dice.Add(new PageDie { data = d });
        }
    }
    public PageDie GetCurrentDie() => IsFinished ? null : dice[currentIndex];
    public void Advance() => currentIndex++;
}