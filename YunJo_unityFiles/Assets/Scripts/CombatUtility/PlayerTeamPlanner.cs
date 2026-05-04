using UnityEngine;
using System.Collections.Generic;

public class PlayerTeamPlanner : MonoBehaviour
{
    public List<CharacterDeck> decks;
    public List<CharacterUnit> enemies;
    public BattleFlowController flow;

    public void Plan()
    {
        foreach (var deck in decks)
        {
            Card card = deck.Draw();

            CharacterUnit target = FindTargetFor(deck.owner);

            flow.QueuePreview(deck.owner, target, card);
        }
    }

    CharacterUnit FindTargetFor(CharacterUnit user)
    {
        if (enemies == null || enemies.Count == 0)
            return null;

        return enemies[Random.Range(0, enemies.Count)];
    }
}