using UnityEngine;
using System.Collections;

public class BattleStarter : MonoBehaviour
{
    public CharacterDeck playerDeck;
    public CharacterDeck enemyDeck;

    public BattleFlowController flow;

    void Start()
    {
        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle()
    {
        if (playerDeck == null || enemyDeck == null)
            yield break;

        // Initialize both decks properly
        playerDeck.Init();
        enemyDeck.Init();

        // ensure starting hand is filled correctly
        playerDeck.FillHandToLimit();
        enemyDeck.FillHandToLimit();

        Debug.Log("[BATTLE] Initialized decks");

        yield return new WaitForSeconds(0.2f);
    }
}