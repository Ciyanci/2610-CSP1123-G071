public class CombatDiceRuntime
{
    public CombatDice source;

    public int lastRoll;

    public bool destroyed;

    public CombatDiceRuntime(CombatDice dice)
    {
        source = dice;
    }

    public int Roll()
    {
        lastRoll = source.Roll();
        return lastRoll;
    }

    public DiceData Data => source.data;
}