using UnityEngine;

public class CombatOutcome
{
    public CombatIntent attacker;
    public CombatIntent defender;

    public int hpDamage;
    public int staggerDamage;

    public bool defenderDied;
    public bool defenderStaggered;
}