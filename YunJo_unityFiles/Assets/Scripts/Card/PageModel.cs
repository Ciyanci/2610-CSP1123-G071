using System.Collections.Generic;

[System.Serializable]
public class CombatPage
{
    public CombatIntent intent;

    public List<PageDie> dice = new();

    public int index;

    public bool IsFinished => index >= dice.Count;

    public PageDie GetCurrentDie()
    {
        if (IsFinished) return null;
        return dice[index];
    }

    public void Advance()
    {
        index++;
    }
}