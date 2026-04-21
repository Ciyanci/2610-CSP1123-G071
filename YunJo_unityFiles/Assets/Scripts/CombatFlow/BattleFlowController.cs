using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFlowController : MonoBehaviour
{
    public CombatCamera cam;
    public ClashSystem clashSystem;
    public TurnSystem turnSystem;

    public List<CombatIntent> intents = new();
    List<(CombatIntent, CombatIntent)> clashes = new();

    //adding intent preview
    public void QueueAction(CharacterUnit user, CharacterUnit target, Card card)
    {
        intents.Add(new CombatIntent
        {
            user = user,
            target = target,
            card = card
        });
    }

    //debug ** manual clash test
    public void TestClash(CharacterUnit a, CharacterUnit b, Card cardA, Card cardB)
    {
        if (a == null || b == null || cardA == null || cardB == null)
        {
            Debug.LogError("TestClash received null values!");
            return;
        }

        intents.Clear();

        QueueAction(a, b, cardA);
        QueueAction(b, a, cardB);

        StartCoroutine(ResolveAll());
    }

    //resolution zone brrrr
    public IEnumerator ResolveAll()
    {
        BuildClashes();

        yield return cam.Reset();

        foreach (var c in clashes)
        {
            yield return clashSystem.Resolve(c.Item1, c.Item2);
            c.Item1.resolved = true;
            c.Item2.resolved = true;
        }

        foreach (var i in intents)
        {
            if (!i.resolved)
                yield return ResolveSingle(i);
        }

        Cleanup();

        //signals done
        CombatFlowController.Instance.SetInputEnabled(false);
    }


    IEnumerator AfterClash()
    {
        yield return new WaitForSeconds(0.3f);
    }

    void BuildClashes()
    {
        clashes.Clear();

        foreach (var a in intents)
        {
            foreach (var b in intents)
            {
                if (a == b) continue;

                if (a.user == b.target && a.target == b.user)
                {
                    if (!clashes.Contains((a, b)) && !clashes.Contains((b, a)))
                        clashes.Add((a, b));
                }
            }
        }
    }

    IEnumerator ResolveSingle(CombatIntent i)
    {
        yield return i.user.MoveTo(i.target.clashAnchor.position);

        i.user.PlayAttack();
        i.target.PlayHit();

        int dmg = Random.Range(i.card.min, i.card.max + 1);
        i.target.TakeDamage(dmg);

        yield return new WaitForSeconds(0.2f);
    }

    void Cleanup()
    {
        intents.Clear();
        clashes.Clear();
    }
}
//kill me