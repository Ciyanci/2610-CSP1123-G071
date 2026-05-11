using UnityEngine;
using System.Collections;

public static class DicePairResolver
{
    public static IEnumerator ResolvePair(
        DicePair pair,
        CharacterUnit aUnit,
        CharacterUnit bUnit)
    {
        // UNOPPOSED A
        if (pair.b == null)
        {
            int roll = pair.a.Roll();
            int dmg = Mathf.Max(1, roll + pair.a.Power);

            bUnit.TakeDamage(dmg, pair.a.damageType);

            pair.a.resolved = true;
            yield break;
        }

        // UNOPPOSED B
        if (pair.a == null)
        {
            int roll = pair.b.Roll();
            int dmg = Mathf.Max(1, roll + pair.b.Power);

            aUnit.TakeDamage(dmg, pair.b.damageType);

            pair.b.resolved = true;
            yield break;
        }

        // BOTH PRESENT → CLASH
        int rollA = pair.a.Roll();
        int rollB = pair.b.Roll();

        if (rollA > rollB)
        {
            int dmg = Mathf.Max(1, rollA + pair.a.Power);
            bUnit.TakeDamage(dmg, pair.a.damageType);

            pair.a.resolved = true;
            pair.b.cancelled = true;
        }
        else if (rollB > rollA)
        {
            int dmg = Mathf.Max(1, rollB + pair.b.Power);
            aUnit.TakeDamage(dmg, pair.b.damageType);

            pair.b.resolved = true;
            pair.a.cancelled = true;
        }
        else
        {
            // DRAW → both continue next step
            pair.a.resolved = true;
            pair.b.resolved = true;
        }

        yield return null;
    }
}