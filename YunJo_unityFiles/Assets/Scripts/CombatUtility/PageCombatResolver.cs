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
            CombatDiceRuntime dieA =
                a.GetCurrentDie();

            CombatDiceRuntime dieB =
                b.GetCurrentDie();

            var result =
                DiceClashResolver.Resolve(
                    dieA,
                    dieB);

            yield return new WaitForSeconds(0.4f);

            switch (result)
            {
                case DiceClashResult.Win:

                    ApplyDamage(
                        a.owner,
                        b.owner,
                        dieA);

                    b.Advance();
                    break;

                case DiceClashResult.Lose:

                    ApplyDamage(
                        b.owner,
                        a.owner,
                        dieB);

                    a.Advance();
                    break;

                case DiceClashResult.Draw:

                    a.Advance();
                    b.Advance();
                    break;
            }
        }

        // remaining dice become unopposed
        while (!a.IsFinished)
        {
            var die = a.GetCurrentDie();

            ApplyDamage(
                a.owner,
                b.owner,
                die);

            a.Advance();

            yield return new WaitForSeconds(0.2f);
        }

        while (!b.IsFinished)
        {
            var die = b.GetCurrentDie();

            ApplyDamage(
                b.owner,
                a.owner,
                die);

            b.Advance();

            yield return new WaitForSeconds(0.2f);
        }
    }

    void ApplyDamage(
        CharacterUnit attacker,
        CharacterUnit target,
        CombatDiceRuntime die)
    {
        int dmg =
            Mathf.Max(
                1,
                die.lastRoll + die.Data.power
            );

        target.TakeDamage(
            dmg,
            die.Data.damageType);

        Debug.Log(
            $"[PAGE] {attacker.name} dealt {dmg}");
    }
}