using System.Collections.Generic;

public class CombatPageRuntime
{
    static int NextId = 1;

    public readonly int PageId;

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
        PageId = NextId++;

        this.owner = owner;
        this.target = target;
        this.card = card;

        foreach (var d in card.GetDice())
        {
            dice.Add(new PageDie
            {
                data = d
            });
        }
    }

    public PageDie GetCurrentDie()
    {
        return IsFinished
            ? null
            : dice[currentIndex];
    }

    public void Advance()
    {
        currentIndex++;
    }
}