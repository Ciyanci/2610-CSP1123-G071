using UnityEngine;

public static class CombatOutcomeResolver
{
    public static CombatOutcome Resolve(CombatIntent winner, CombatIntent loser, CombatRoll winRoll, CombatRoll loseRoll)
    {
        int damage = Mathf.Max(1, Mathf.Abs(winRoll.value - loseRoll.value));

        bool staggerHit = winRoll.value > loseRoll.value + 3;

        int staggerDamage = staggerHit ? 10 : 0;

        bool died = false;
        bool staggered = false;

        return new CombatOutcome
        {
            attacker = winner,
            defender = loser,

            hpDamage = damage,
            staggerDamage = staggerDamage,

            defenderDied = died,
            defenderStaggered = staggered
        };
    }
}