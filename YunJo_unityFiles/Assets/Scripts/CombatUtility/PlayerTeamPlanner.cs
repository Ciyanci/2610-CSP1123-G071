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
            if (deck.GetHand().Count == 0)
                continue;

            // pick first available card (or later: UI-selected)
            Card card = deck.GetHand()[0];

            CharacterUnit target = FindTargetFor(deck.owner);

            flow.QueuePreview(deck.owner, target, card);

            deck.UseCard(card); // 🔥 IMPORTANT: now consumes properly
        }
    }

    CharacterUnit FindTargetFor(CharacterUnit user)
    {
        if (enemies == null || enemies.Count == 0)
            return null;

        return enemies[Random.Range(0, enemies.Count)];
    }
}