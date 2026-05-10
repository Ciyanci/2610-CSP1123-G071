using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CombatResolver : MonoBehaviour
{
    public PageCombatResolver pageResolver;

    public IEnumerator Resolve(CombatTurnContext ctx)
    {
        var ordered = ctx.intents
            .OrderByDescending(i => i.priority)
            .ToList();

        HashSet<CombatIntent> resolved = new();

        // CLASHES
        foreach (var clash in ctx.clashes)
        {
            yield return ResolveClash(clash.a, clash.b);
            resolved.Add(clash.a);
            resolved.Add(clash.b);
        }

        // UNOPPOSED
        foreach (var intent in ordered)
        {
            if (resolved.Contains(intent))
                continue;

            yield return ResolveUnopposed(intent);
        }
    }

    IEnumerator ResolveClash(CombatIntent a, CombatIntent b)
    {
        yield return pageResolver.ResolvePages(
            a.CreatePage(),
            b.CreatePage()
        );
    }

    IEnumerator ResolveUnopposed(CombatIntent intent)
    {
        var page = intent.CreatePage();

        while (!page.IsFinished)
        {
            var die = page.GetCurrentDie();
            int roll = die.Roll();

            int dmg = Mathf.Max(1, roll + die.Data.power);

            intent.target.TakeDamage(dmg, die.Data.damageType);

            page.Advance();
            yield return new WaitForSeconds(0.25f);
        }
    }
}