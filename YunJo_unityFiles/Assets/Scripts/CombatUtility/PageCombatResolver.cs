using UnityEngine;
using System.Collections;

public class PageCombatResolver : MonoBehaviour
{
    public IEnumerator ResolvePages(
        CombatPageRuntime a,
        CombatPageRuntime b)
    {
        while (!a.IsFinished && !b.IsFinished)
        {
            CombatDiceRuntime dieA = a.GetCurrentDie();
            CombatDiceRuntime dieB = b.GetCurrentDie();

            if (dieA == null || dieB == null)
                yield break;

            var result = DiceClashResolver.Resolve(dieA, dieB);

            yield return new WaitForSeconds(0.4f);

            switch (result)
            {
                case DiceClashResult.Win:
                    ApplyDamage(a.owner, b.owner, dieA);
                    b.Advance();
                    break;

                case DiceClashResult.Lose:
                    ApplyDamage(b.owner, a.owner, dieB);
                    a.Advance();
                    break;

                case DiceClashResult.Draw:
                    a.Advance();
                    b.Advance();
                    break;
            }
        }

        // =========================
        // UNOPPOSED A
        // =========================
        while (!a.IsFinished)
        {
            var die = a.GetCurrentDie();
            if (die == null) break;

            ApplyDamage(a.owner, b.owner, die);
            a.Advance();

            yield return new WaitForSeconds(0.2f);
        }

        // =========================
        // UNOPPOSED B
        // =========================
        while (!b.IsFinished)
        {
            var die = b.GetCurrentDie();
            if (die == null) break;

            ApplyDamage(b.owner, a.owner, die);
            b.Advance();

            yield return new WaitForSeconds(0.2f);
        }
    }

    void ApplyDamage(
        CharacterUnit attacker,
        CharacterUnit target,
        CombatDiceRuntime die)
    {
        if (target == null || die == null)
            return;

        int dmg = Mathf.Max(1, die.lastRoll + die.Data.power);

        target.TakeDamage(dmg, die.Data.damageType);

        Debug.Log($"[PAGE] {attacker.name} dealt {dmg}");
    }
}