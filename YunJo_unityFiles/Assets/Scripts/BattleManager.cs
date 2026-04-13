using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public List<CharacterUnit> allies;
    public List<CharacterUnit> enemies;

    public ClashSystem clash;
    public DiceUI diceUI;

    List<CombatAction> actions = new();

    void Start()
    {
        StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        while (true)
        {
            yield return ExecuteTurn();
        }
    }

    IEnumerator ExecuteTurn()
    {
        actions.Clear();

        if (enemies.Count == 0 || allies.Count == 0)
            yield break;

        foreach (var u in allies) u.ResetState();
        foreach (var u in enemies) u.ResetState();

        foreach (var a in allies)
            actions.Add(Create(a, enemies[Random.Range(0, enemies.Count)]));

        foreach (var e in enemies)
            actions.Add(Create(e, allies[Random.Range(0, allies.Count)]));

        actions.Sort((a, b) => b.speed.CompareTo(a.speed));

        for (int i = 0; i < actions.Count; i++)
        {
            for (int j = i + 1; j < actions.Count; j++)
            {
                if (!actions[i].resolved &&
                    actions[i].target == actions[j].user &&
                    actions[j].target == actions[i].user)
                {
                    yield return clash.Resolve(actions[i], actions[j]);
                    break;
                }
            }
        }

        foreach (var a in actions)
        {
            if (a.resolved) continue;
            yield return StartCoroutine(ResolveAttack(a));
        }
    }

    IEnumerator ResolveAttack(CombatAction a)
    {
        CharacterUnit A = a.user;
        CharacterUnit B = a.target;

        yield return A.MoveTo(A.weaponAnchor.position);
        yield return A.WindUp(0.1f);

        A.PlayAttack();

        int roll = Random.Range(a.card.min, a.card.max + 1);

        yield return new WaitForSeconds(0.1f);

        B.PlayHit();
        B.TakeDamage(a.card.damage * roll / a.card.max);

        yield return A.MoveTo(A.clashAnchor.position);
    }

    CombatAction Create(CharacterUnit u, CharacterUnit t)
    {
        return new CombatAction
        {
            user = u,
            target = t,
            card = new Card { min = 1, max = 6, damage = 10 },
            speed = Random.Range(1, 10)
        };
    }
}