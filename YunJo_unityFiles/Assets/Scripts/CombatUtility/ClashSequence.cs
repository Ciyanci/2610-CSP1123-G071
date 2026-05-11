[System.Serializable]
public class ClashSequence
{
    public CombatIntent a;
    public CombatIntent b;

    public int index;

    public bool IsFinished =>
        index >= Mathf.Max(a.card.DiceCount, b.card.DiceCount);
}