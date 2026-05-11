using System.Collections;
using UnityEngine;

public static class ClashResolver
{
    public static IEnumerator Resolve(ClashPair pair, System.Action<ClashResult> onComplete)
    {
        int aRoll = Roll(pair.a);
        int bRoll = Roll(pair.b);

        yield return new WaitForSeconds(0.1f);

        if (aRoll == bRoll)
            yield break;

        var winner = aRoll > bRoll ? pair.a : pair.b;
        var loser = aRoll > bRoll ? pair.b : pair.a;

        int damage = Mathf.Max(1, Mathf.Abs(aRoll - bRoll));

        loser.target.TakeDamage(damage, DamageType.Slash);

        if (loser.target.hp <= 0)
            loser.target.Die();

        if (loser.target.stagger <= 0)
            loser.target.ApplyStagger();

        onComplete?.Invoke(new ClashResult
        {
            winner = winner,
            loser = loser
        });

        yield return new WaitForSeconds(0.15f);
    }

    static int Roll(CombatIntent intent)
    {
        return Random.Range(1, 7) + intent.priority;
    }
}