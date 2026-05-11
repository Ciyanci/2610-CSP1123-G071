public class CombatRoll
{
    public int value;

    public CombatIntent owner;
    public int TotalValue => value + owner.card.power;
}