using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public CardDeck deck;
    public TargetingSystem targeting;
    public float thinkTime = 0.5f;

    CharacterUnit self;

    void Awake()
    {
        self = GetComponent<CharacterUnit>();
    }

    public IEnumerator TakeTurn()
    {
        yield return new WaitForSeconds(thinkTime);

        if (deck == null || deck.cards == null || deck.cards.Count == 0)
            yield break;

        self.RefreshEnergy();

        int safety = 10;

        while (self.currentEnergy > 0 && safety-- > 0)
        {
            Card card = GetPlayableCard();
            if (card == null) yield break;

            if (!self.CanPay(card.Cost))
                break;

            CharacterUnit target = PickTarget();
            if (target == null)
                yield break;

            self.Spend(card.Cost);

            // ✅ IMPORTANT CHANGE (RUINA STYLE)
            CombatFlowController.Instance.SelectCard(card, self);
            CombatInputController.Instance.SelectTarget(target);

            yield return new WaitForSeconds(0.2f);
        }
    }

    Card GetPlayableCard()
    {
        List<Card> valid = new();

        foreach (var c in deck.cards)
            if (c.Cost <= self.currentEnergy)
                valid.Add(c);

        if (valid.Count > 0)
            return valid[Random.Range(0, valid.Count)];

        return deck.cards.Find(c => c.Cost == 0);
    }

    CharacterUnit PickTarget()
    {
        if (targeting == null || targeting.enemies == null || targeting.enemies.Count == 0)
            return null;

        return targeting.enemies[Random.Range(0, targeting.enemies.Count)];
    }
}