using UnityEngine;
using System.Collections;

public class BattleStarter : MonoBehaviour
{
    public CharacterDeck playerDeck;
    public CharacterDeck enemyDeck;


    void Start()
    {
        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle()
    {
        UnitRegistry.Instance.Refresh();
        if (playerDeck == null || enemyDeck == null)
            yield break;

        //initialise both decks
        playerDeck.Init();
        enemyDeck.Init();

        CombatHUDController.Instance?.Bind();

        //ensure starting hand is filled correctly
        playerDeck.FillHandToLimit();
        enemyDeck.FillHandToLimit();

        Debug.Log("[BATTLE] Initialized decks");
        CombatInfoBar.Instance?.ShowDefault();
        CombatFlowController.Instance.SetInputEnabled(true);

        yield return new WaitForSeconds(0.2f);
    }
}