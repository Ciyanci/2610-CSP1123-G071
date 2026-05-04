using UnityEngine;
using System.Collections;

public class BattleStarter : MonoBehaviour
{
    public CharacterDeck playerDeck;
    public CharacterDeck enemyDeck;
    public BattleFlowController flow;

    void Start(){}
    IEnumerator StartBattle()
    {
        //give initial hand
        for (int i = 0; i < 5; i++)
        {
            playerDeck.Draw();
            enemyDeck.Draw();
            yield return new WaitForSeconds(0.1f);
        }
    }
}