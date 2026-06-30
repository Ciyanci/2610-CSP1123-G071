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
        if (!self.CanAct) yield break;

        yield return new WaitForSeconds(thinkTime);

        self.RefreshLight();

        //cap to allowed slots
        int allowedSlots = self.GetSpeedDiceCount();
        int assigned = 0;
        int safety = 10;

        while (self.currentLight > 0 && assigned < allowedSlots && safety-- > 0)
        {
            Card card = GetPlayableCard();
            if (card == null) yield break;

            if (!self.CanPay(card.Cost)) break;

            CharacterUnit target = FindRandomPlayer();
            if (target == null) yield break;

            SpeedSlot slot = self.GetHighestAvailableSlot();
            if (slot == null) yield break;

            ActionPlanner.AssignToSlot(self, slot, card, target);
            ArrowManager.Instance?.AddPlannedArrow(slot);
            slot.ui?.Refresh();

            assigned++; //tracks how many slots are filled

            yield return new WaitForSeconds(0.15f);
        }

        foreach (var slot in self.speedSlots)
        {
            if (slot.state == SlotState.Planned)
            {
                slot.Commit();
                Debug.Log($"[AI] {self.unitName} committed slot spd:{slot.value} card:{slot.assignedCard?.Name}");
            }
            else
            {
                Debug.Log($"[AI] {self.unitName} slot skipped (state:{slot.state})");
            }
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
        var players = FindObjectsByType<CharacterUnit>(FindObjectsInactive.Exclude);

        List<CharacterUnit> valid = new();

        foreach (var p in players)
        {
            if (!p.CompareTag("Player"))
                continue;

            if (p.IsDead)
                continue;

            valid.Add(p);
        }
        return valid.Count > 0
            ? valid[Random.Range(0, valid.Count)]
            : null;
    }
}