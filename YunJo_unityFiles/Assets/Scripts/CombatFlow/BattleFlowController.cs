using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFlowController : MonoBehaviour
{
    public CombatCamera cam;
    public ClashSystem clashSystem;

    public List<ArrowController> activeArrows = new();
    public ArrowController arrowPrefab;

    public List<CombatIntent> intents = new();
    List<(CombatIntent, CombatIntent)> clashes = new();

    // =========================
    // INTENT (supports multiple)
    // =========================
    public void QueueAction(CharacterUnit user, CharacterUnit target, Card card)
    {
        CombatIntent intent = new CombatIntent
        {
            user = user,
            target = target,
            card = card
        };

        intents.Add(intent);

        ArrowController arrow = Instantiate(arrowPrefab, transform);
        arrow.Set(user.headAnchor, target.headAnchor);

        activeArrows.Add(arrow);
    }

    // =========================
    // TEST DRIVER SAFE ENTRY
    // =========================
    public void TestClash(List<CharacterUnit> units, List<Card> cards)
    {
        intents.Clear();
        CleanupArrows();

        int count = Mathf.Min(units.Count, cards.Count);

        for (int i = 0; i < count; i++)
        {
            var user = units[i];
            var target = units[(i + 1) % count];

            QueueAction(user, target, cards[i]);
        }

        StartCoroutine(ResolveAll());
    }

    // =========================
    // RESOLVE ALL (MULTI + UNOPPOSED)
    // =========================
    public IEnumerator ResolveAll()
    {
        BuildClashes();

        yield return cam.Reset();

        // 1. RESOLVE CLASHES
        foreach (var c in clashes)
        {
            yield return clashSystem.Resolve(c.Item1, c.Item2);

            c.Item1.resolved = true;
            c.Item2.resolved = true;
        }

        // 2. UNOPPOSED ATTACKS
        foreach (var i in intents)
        {
            if (!i.resolved)
            {
                yield return ResolveSingle(i);
            }
        }

        Cleanup();

        CombatFlowController.Instance.SetInputEnabled(false);
    }

    IEnumerator ResolveSingle(CombatIntent i)
    {
        yield return i.user.MoveTo(i.target.clashAnchor.position);

        i.user.PlayAttack();
        i.target.PlayHit();

        int dmg = Random.Range(i.card.min, i.card.max + 1);
        i.target.TakeDamage(dmg);

        yield return new WaitForSeconds(0.15f);
    }

    // =========================
    // CLASH BUILDING (safe multi-match)
    // =========================
    void BuildClashes()
    {
        clashes.Clear();

        for (int i = 0; i < intents.Count; i++)
        {
            for (int j = i + 1; j < intents.Count; j++)
            {
                var a = intents[i];
                var b = intents[j];

                if (a.user == b.target && a.target == b.user)
                {
                    clashes.Add((a, b));
                }
            }
        }
    }

    void Cleanup()
    {
        intents.Clear();
        clashes.Clear();
        CleanupArrows();
    }

    void CleanupArrows()
    {
        foreach (var arrow in activeArrows)
        {
            if (arrow != null)
                Destroy(arrow.gameObject);
        }

        activeArrows.Clear();
    }
}
//kill me