using UnityEngine;

public static class CombatDiceUtility
{
    public static CombatRoll Roll(CombatIntent intent)
    {
        return new CombatRoll
        {
            owner = intent,
            value = Random.Range(1, 7)
        };
    }
}