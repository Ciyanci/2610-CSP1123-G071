using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public CharacterDeck deck;
    public float thinkTime = 0.5f;

    CharacterUnit self;

    void Awake()
    {
        self = GetComponent<CharacterUnit>();
    }

    public IEnumerator TakeTurn()
    {
        yield return new WaitForSeconds(thinkTime);

        self.RefreshLight();

        int safety = 10;

        while (self.currentLight > 0 && safety-- > 0)
        {
            Card card = GetPlayableCard();
            if (card == null) yield break;

            if (!self.CanPay(card.Cost))
                break;

            CharacterUnit target = FindRandomPlayer();
            if (target == null) yield break;

            SpeedSlot slot = self.GetHighestAvailableSlot();
            if (slot == null) yield break;

            self.SpendLight(card.Cost);

            slot.Assign(card, target, self);

            yield return new WaitForSeconds(0.15f);
        }
    }

    Card GetPlayableCard()
    {
        List<Card> valid = new();

        foreach (var c in deck.GetHand())
        {
            if (c.Cost <= self.currentLight)
                valid.Add(c);
        }

        return valid.Count > 0
            ? valid[Random.Range(0, valid.Count)]
            : null;
    }

    CharacterUnit FindRandomPlayer()
    {
        var players = FindObjectsByType<CharacterUnit>(FindObjectsSortMode.None);

        List<CharacterUnit> valid = new();

        foreach (var p in players)
            if (p.CompareTag("Player"))
                valid.Add(p);

        return valid.Count > 0
            ? valid[Random.Range(0, valid.Count)]
            : null;
    }
}