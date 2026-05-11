using UnityEngine;

[System.Serializable]
public class PageDie
{
    public DiceData data;

    public CombatIntent owner;

    public int roll;

    public bool resolved;
    public bool cancelled;

    public bool IsResolved => resolved;

    public int Roll()
    {
        roll = Random.Range(data.minRoll, data.maxRoll + 1);
        return roll;
    }

    public int Power => data.power;

    public DamageType damageType => data.damageType;

    public void MarkResolved()
    {
        resolved = true;
    }
}