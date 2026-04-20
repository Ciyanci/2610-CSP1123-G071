using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BattleFlowController : MonoBehaviour
{
    public CombatCamera cam;

    List<CombatIntent> intents = new();

    List<(CombatIntent, CombatIntent)> clashes = new();

    public void RegisterIntent(CharacterUnit user, Card card, CharacterUnit target)
    {
        intents.Add(new CombatIntent
        {
            user = user,
            card = card,
            target = target
        });
    }

    // -------------------------
    // PHASE 1: SPEED ROLL
    // -------------------------
    public IEnumerator RollSpeedPhase()
    {
        foreach (var i in intents)
        {
            i.speed = DiceSystem.Roll(1, 6);
            yield return i.user.diceUI.Roll(1, 6, i.user.headAnchor, r => i.speed = r);
        }

        intents.Sort((a, b) => b.speed.CompareTo(a.speed));
    }

    // -------------------------
    // PHASE 2: CLASH PAIRING
    // -------------------------
    public void BuildClashes()
    {
        clashes.Clear();

        for (int i = 0; i < intents.Count; i++)
        {
            for (int j = i + 1; j < intents.Count; j++)
            {
                if (intents[i].target == intents[j].user &&
                    intents[j].target == intents[i].user)
                {
                    clashes.Add((intents[i], intents[j]));
                }
            }
        }
    }

    // -------------------------
    // PHASE 3: RESOLVE
    // -------------------------
    public IEnumerator Resolve()
    {
        foreach (var clash in clashes)
        {
            yield return ResolveClash(clash.Item1, clash.Item2);
        }

        foreach (var i in intents)
        {
            if (!IsInClash(i))
                yield return ResolveSingle(i);
        }

        Cleanup();
    }

    IEnumerator ResolveClash(CombatIntent a, CombatIntent b)
    {
        yield return cam.ClashZoom((a.user.transform.position + b.user.transform.position) / 2f);

        int aRoll = DiceSystem.Roll(1, 6);
        int bRoll = DiceSystem.Roll(1, 6);

        if (aRoll >= bRoll)
            yield return ResolveWin(a, b);
        else
            yield return ResolveWin(b, a);

        yield return cam.Reset();
    }

    IEnumerator ResolveWin(CombatIntent winner, CombatIntent loser)
    {
        yield return winner.user.MoveTo(loser.user.clashAnchor.position);

        winner.user.PlayAttack();
        loser.user.PlayHit();

        int dmg = winner.card.damage + DiceSystem.Roll(1, 6);
        loser.user.TakeDamage(dmg);
    }

    IEnumerator ResolveSingle(CombatIntent i)
    {
        yield return i.user.MoveTo(i.target.clashAnchor.position);

        i.user.PlayAttack();
        i.target.PlayHit();

        int dmg = i.card.damage + DiceSystem.Roll(1, 6);
        i.target.TakeDamage(dmg);
    }

    bool IsInClash(CombatIntent i)
    {
        foreach (var c in clashes)
            if (c.Item1 == i || c.Item2 == i)
                return true;
        return false;
    }

    void Cleanup()
    {
        intents.Clear();
        clashes.Clear();
    }
}

public class CombatIntent
{
    public CharacterUnit user;
    public CharacterUnit target;
    public Card card;
    public int speed;
}
//need to add assets first before ts can work