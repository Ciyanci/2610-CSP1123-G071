[System.Serializable]
public class ClashPair
{
    public CombatIntent a;
    public CombatIntent b;

    public bool resolved;

    public ClashPair(CombatIntent a, CombatIntent b)
    {
        this.a = a;
        this.b = b;
        resolved = false;
    }
}