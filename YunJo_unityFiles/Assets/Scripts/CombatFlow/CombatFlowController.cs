using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatFlowController : MonoBehaviour
{
    public static CombatFlowController Instance;

    [Header("State")]
    public bool inputEnabled;

    Card selectedCard;

    CharacterUnit selectedUser;
    CharacterUnit selectedUnit;

    [Header("Visuals")]
    public ArrowController arrow;

    void Awake()
    {
        Instance = this;
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;

        Debug.Log($"[FLOW] Input Enabled: {enabled}");
    }

    public void SelectUnit(CharacterUnit unit)
    {
        selectedUnit = unit;

        if (unit.deck != null)
        {
            HandUI.Instance.Show(unit.deck);
        }
    }

    public void SelectCard(Card card, CharacterUnit user)
    {
        if (!inputEnabled || card == null || user == null)
            return;

        selectedCard = card;
        selectedUser = user;

        Debug.Log($"[SELECT] {user.unitName} selected {card.Name}");

        arrow?.Begin(user, card);
    }

    public void StartTargeting(
        Card card,
        CharacterUnit user)
    {
        SelectCard(card, user);
    }

    public void ConfirmTarget(CharacterUnit target)
    {
        if (!inputEnabled ||
            selectedCard == null ||
            selectedUser == null ||
            target == null ||
            target == selectedUser)
            return;

        PreviewManager.Instance.QueuePreview(
            selectedUser,
            target,
            selectedCard
        );

        selectedUser.deck.UseCard(selectedCard);

        HandUI.Instance.Refresh(selectedUser.deck);

        arrow?.End();

        ClearSelection();
    }

    public void ConfirmPlanning()
    {
        if (!inputEnabled)
            return;

        StartCoroutine(
            BattleFlowController.Instance.ResolveTurn()
        );

        SetInputEnabled(false);
    }

    public void AutoAssignPlayerActions()
    {
        foreach (var player in UnitRegistry.Instance.players)
        {
            if (player.deck == null)
                continue;

            List<Card> hand = player.deck.GetHand();

            if (hand.Count == 0)
                continue;

            Card randomCard =
                hand[Random.Range(0, hand.Count)];

            if (UnitRegistry.Instance.enemies.Count == 0)
                continue;

            CharacterUnit target =
                UnitRegistry.Instance.enemies[
                    Random.Range(
                        0,
                        UnitRegistry.Instance.enemies.Count
                    )
                ];

            PreviewManager.Instance.QueuePreview(
                player,
                target,
                randomCard
            );

            player.deck.UseCard(randomCard);
        }

        HandUI.Instance.Refresh(selectedUnit.deck);
    }

    public void ClearSelection()
    {
        selectedCard = null;
        selectedUser = null;
    }

    public void ResetAll()
    {
        if (arrow != null)
        {
            arrow.End();
        }

        ClearSelection();
    }
}