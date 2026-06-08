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
        yield return null;

        if (playerDeck == null || enemyDeck == null) yield break;

        playerDeck.Init();
        enemyDeck.Init();

        CombatHUDController.Instance?.Bind();

        CombatAudioManager.Instance?.PlayTurnBegin();

        Debug.Log("[BATTLE] Initialized — state machine takes over");
    }
}