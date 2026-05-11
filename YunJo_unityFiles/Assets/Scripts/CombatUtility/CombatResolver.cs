using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CombatResolver : MonoBehaviour
{
    public PageCombatResolver pageResolver;

    // =========================
    // MAIN TURN RESOLUTION
    // =========================
    public IEnumerator Resolve(CombatTurnContext ctx)
    {
        var ordered = ctx.intents
            .OrderByDescending(i => i.priority)
            .ToList();

        HashSet<CombatIntent> resolved = new();

        // =========================
        // CLASHES
        // =========================
        foreach (var clash in ctx.clashes)
        {
            if (!CombatIntentUtility.IsValid(clash.a) ||
                !CombatIntentUtility.IsValid(clash.b))
                continue;

            // INTERRUPT CHECK (stagger system)
            if (clash.a.user.isInterrupted ||
                clash.b.user.isInterrupted)
            {
                clash.a.speedSlot?.Execute();
                clash.b.speedSlot?.Execute();

                resolved.Add(clash.a);
                resolved.Add(clash.b);
                continue;
            }

            yield return ResolveClash(clash.a, clash.b);

            resolved.Add(clash.a);
            resolved.Add(clash.b);
        }

        // =========================
        // UNOPPOSED
        // =========================
        foreach (var intent in ordered)
        {
            if (resolved.Contains(intent))
                continue;

            if (!CombatIntentUtility.IsValid(intent))
                continue;

            if (intent.user.isInterrupted)
                continue;

            yield return ResolveUnopposed(intent);

            intent.speedSlot?.Execute();
        }
    }

    // =========================================================
    // MULTI-DICE CLASH ENGINE
    // =========================================================
    IEnumerator ResolveClash(CombatIntent a, CombatIntent b)
    {
        int max = Mathf.Max(
            a.card.DiceCount,
            b.card.DiceCount
        );

        for (int i = 0; i < max; i++)
        {
            if (a.user.IsDead || b.user.IsDead)
                yield break;

            if (a.user.isInterrupted || b.user.isInterrupted)
                yield break;

            DiceData dieA = a.card.GetDiceSafe(i);
            DiceData dieB = b.card.GetDiceSafe(i);

            // =========================
            // BOTH DICE PRESENT
            // =========================
            if (dieA != null && dieB != null)
            {
                int rollA = dieA.Roll();
                int rollB = dieB.Roll();

                CharacterUnit winner = null;
                CharacterUnit loser = null;
                DiceData winDie = null;

                if (rollA > rollB)
                {
                    winner = a.user;
                    loser = b.user;
                    winDie = dieA;
                }
                else if (rollB > rollA)
                {
                    winner = b.user;
                    loser = a.user;
                    winDie = dieB;
                }

                if (winner != null)
                {
                    ApplyDieDamage(winner, loser, winDie);
                }
            }

            // =========================
            // A EXTRA DICE
            // =========================
            else if (dieA != null)
            {
                ApplyDieDamage(a.user, b.user, dieA);
            }

            // =========================
            // B EXTRA DICE
            // =========================
            else if (dieB != null)
            {
                ApplyDieDamage(b.user, a.user, dieB);
            }

            yield return new WaitForSeconds(0.25f);

            if (a.user.IsDead || b.user.IsDead)
                yield break;

            if (a.user.isInterrupted || b.user.isInterrupted)
                yield break;
        }
    }

    // =========================================================
    // DIE RESOLUTION CORE
    // =========================================================
    void ApplyDieDamage(
        CharacterUnit attacker,
        CharacterUnit defender,
        DiceData die)
    {
        if (attacker == null || defender == null || die == null)
            return;

        int roll = Random.Range(die.minRoll, die.maxRoll + 1);

        int rawDamage = Mathf.Max(1, roll + die.power);

        int finalDamage = ApplyDefense(defender, rawDamage);

        if (finalDamage <= 0)
            return;

        defender.TakeDamage(finalDamage, die.damageType);

        if (defender.state == UnitState.Staggered)
        {
            defender.isInterrupted = true;
        }
    }
    // =========================================================
    // DEFENSE SYSTEM
    // =========================================================
    int ApplyDefense(CharacterUnit defender, int incomingDamage)
    {
        DefensiveDie defense = defender.GetAvailableDefense();

        if (defense == null)
            return incomingDamage;

        int roll = defense.Roll();

        switch (defense.type)
        {
            case DefenseType.Block:
            {
                int reduced = Mathf.Max(0, incomingDamage - roll);
                Debug.Log($"[BLOCK] {defender.unitName}: {incomingDamage} → {reduced}");
                return reduced;
            }

            case DefenseType.Evade:
            {
                if (roll >= incomingDamage)
                {
                    Debug.Log($"[EVADE] {defender.unitName} fully dodged");
                    return 0;
                }
                return incomingDamage;
            }
        }

        return incomingDamage;
    }

    // =========================================================
    // UNOPPOSED PAGE SYSTEM
    // =========================================================
    IEnumerator ResolveUnopposed(CombatIntent intent)
    {
        CombatPageRuntime page = intent.CreatePage();

        while (!page.IsFinished)
        {
            if (intent.user.isInterrupted)
                yield break;

            PageDie die = page.GetCurrentDie();
            if (die == null)
                yield break;

            int roll = die.Roll();
            int dmg = Mathf.Max(1, roll + die.Power);

            int final = ApplyDefense(intent.target, dmg);

            if (final > 0)
                intent.target.TakeDamage(final, die.damageType);

            die.resolved = true;

            page.Advance();

            yield return new WaitForSeconds(0.25f);

            if (intent.target.IsDead)
                break;
        }
    }
}