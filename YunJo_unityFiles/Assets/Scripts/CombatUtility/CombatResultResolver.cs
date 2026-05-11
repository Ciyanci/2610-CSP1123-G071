using UnityEngine;

public static class CombatResultResolver
{
    public static CombatOutcome Resolve(CombatIntent winner, CombatIntent loser)
    {
        if (winner == null || loser == null)
            return null;

        int baseDamage = GetCardPower(winner.card);

        int finalDamage = Mathf.Max(1, baseDamage);

        loser.target.TakeDamage(finalDamage, GetDamageType(winner.card));

        bool died = loser.target.hp <= 0;
        bool staggered = loser.target.stagger <= 0;

        return new CombatOutcome
        {
            attacker = winner,
            defender = loser,
            hpDamage = finalDamage,
            defenderDied = died,
            defenderStaggered = staggered
        };
    }

    static int GetCardPower(Card card)
    {
        // SAFE FALLBACK (since Card.Power does NOT exist yet)
        if (card == null) return 1;

        return card.Cost + 1; 
        // temporary placeholder scaling
        // later: card.Data.power or skill table
    }

    static DamageType GetDamageType(Card card)
    {
        // fallback until card system defines types
        return DamageType.Slash;
    }
}