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
    public void PlayCardFromUI(Card c)
    {
        if (targeting.selectedTarget == null) return;

        // queue player action instead of resolving instantly
        playerQueue.Add(new CombatAction
        {
            user = playerUnit,
            target = targeting.selectedTarget,
            card = c
        });

        // enemy also queues action bookfgoagoag
        enemyQueue.Add(new CombatAction
        {
            user = targeting.selectedTarget,
            target = playerUnit,
            card = playerDeck.Draw()
        });

        Debug.Log("Action queued");
    }

    public void ResolveTurn()
    {
        StartCoroutine(ResolveRoutine());
    }

    IEnumerator ResolveRoutine()
    {
        // resolve each clash pair
        for (int i = 0; i < playerQueue.Count; i++)
        {
            yield return clash.Resolve(playerQueue[i], enemyQueue[i]);
        }

        // clear queues after turn
        playerQueue.Clear();
        enemyQueue.Clear();
    }
}

//need to add assets first before ts can work