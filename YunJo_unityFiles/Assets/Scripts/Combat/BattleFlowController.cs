using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFlowController : MonoBehaviour
{
    public ClashSystem clash;
    public CardDeck playerDeck;
    public TargetingSystem targeting;

    CharacterUnit playerUnit;

    // queue
    List<CombatAction> playerQueue = new();
    List<CombatAction> enemyQueue = new();

    void Start()
    {
        playerUnit = FindFirstObjectByType<CharacterUnit>();
    }

    // called when clicking acrd
    public void QueueAction(CharacterUnit user, CharacterUnit target, Card c)
    {
        playerQueue.Add(new CombatAction
        {
            user = user,
            target = target,
            card = c,
            speed = Random.Range(1, 10)
        });

        enemyQueue.Add(new CombatAction
        {
            user = target,
            target = user,
            card = playerDeck.Draw(),
            speed = Random.Range(1, 10)
        });
    }

    public void ResolveTurn()
    {
        StartCoroutine(ResolveRoutine());
    }

    void ResetUnits()
    {
        foreach (var u in FindObjectsByType<CharacterUnit>(FindObjectsSortMode.None))
        {
            u.ResetState();
        }
    }

    IEnumerator ResolveOneSided(CombatAction action)
    {
        CharacterUnit user = action.user;
        CharacterUnit target = action.target;

        // movement (melee only)
        if (user.unitType == UnitType.Melee)
            yield return user.MoveTo(target.clashAnchor.position);

        user.PlayAttack();
        target.PlayHit();

        Vector3 dir = (target.visual.position - user.visual.position).normalized;

        yield return target.Recoil(dir, 0.3f, 0.15f);

        int dmg = Mathf.RoundToInt(
            action.card.damage * Random.Range(0.5f, 1f)
        );

        target.TakeDamage(dmg);

        yield return new WaitForSeconds(0.2f);
    }

    IEnumerator ResolveRoutine()
    {
        // resolve each clash pair
        playerQueue.Sort((a, b) => b.speed.CompareTo(a.speed));
        enemyQueue.Sort((a, b) => b.speed.CompareTo(a.speed));
        for (int i = 0; i < playerQueue.Count; i++)
        {
            CombatAction playerAction = playerQueue[i];
            CombatAction enemyAction = enemyQueue[i];

            bool isClash =
                playerAction.target == enemyAction.user &&
                enemyAction.target == playerAction.user;

            if (isClash)
            {
                // 🔥 BOTH TARGET EACH OTHER → CLASH
                yield return clash.Resolve(playerAction, enemyAction);
            }
            else
            {
                // ⚡ SPEED REDIRECTION LOGIC

                if (playerAction.speed >= enemyAction.speed)
                {
                    // player is faster → attack first
                    yield return ResolveOneSided(playerAction);
                    yield return ResolveOneSided(enemyAction);
                }
                else
                {
                    // enemy is faster
                    yield return ResolveOneSided(enemyAction);
                    yield return ResolveOneSided(playerAction);
                }
            }
        }

        // clear queues after turn
        playerQueue.Clear();
        enemyQueue.Clear();
        yield return FindFirstObjectByType<TurnSystem>().NextTurn();
        ResetUnits();
    }
}

//need to add assets first before ts can work