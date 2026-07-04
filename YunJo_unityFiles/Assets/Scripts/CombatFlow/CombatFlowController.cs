using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatFlowController : MonoBehaviour
{
    public static CombatFlowController Instance;

    [Header("State")]
    public bool inputEnabled;

    Card          selectedCard;
    CharacterUnit selectedUser;
    SpeedSlot     selectedSlot;

    public bool IsTargeting => selectedCard != null;

    void Awake()
    {
        Instance = this;
    }

    //card selection
    public void StartTargeting(Card card, CharacterUnit user)
    {
        if (!inputEnabled || card == null) return;

        CancelSelection();

        selectedCard = card;
        selectedUser = user;
        selectedSlot = null;

        Debug.Log($"[FLOW] Card selected: {card.Name} — pick a speed slot");
    }

    //slot selection
    public void SelectSlot(SpeedSlot slot)
    {
        if (!inputEnabled)        return;
        if (selectedCard == null) return;
        if (slot?.owner == null)  return;

        if (!UnitRegistry.Instance.players.Contains(slot.owner)) return;

        if (slot.state == SlotState.Committed ||
            slot.state == SlotState.Executed)  return;

        if (slot.state == SlotState.Planned && slot.assignedCard != null)
        {
            slot.owner.currentLight = Mathf.Min(
                slot.owner.currentLight + slot.assignedCard.Cost,
                slot.owner.maxLight);

            slot.owner.RefreshLight();

            slot.owner.deck?.ReturnToHand(slot.assignedCard);

            ArrowManager.Instance?.RemovePlannedArrow(slot);

            slot.Clear();
        }

        selectedSlot?.ui?.SetSelected(false);
        selectedSlot = slot;
        slot.ui?.SetSelected(true);

        Debug.Log($"[FLOW] Slot selected: value {slot.value} — pick a target");
    }

    //target confirm
    public void ConfirmTarget(CharacterUnit target)
    {
        if (selectedCard == null || selectedUser == null) return;
        if (target == null || target.IsDead) return;
        CombatAudioManager.Instance?.PlayTargetSelect();
        SpeedSlot slot = selectedSlot ?? selectedUser.GetHighestAvailableSlot();
        if (slot == null)
        {
            Debug.Log("[FLOW] No available slot");
            CancelSelection();
            return;
        }
        //AssignToSlot now returns false if unit can't afford the card
        bool assigned = ActionPlanner.AssignToSlot(
            selectedUser, slot, selectedCard, target);
        if (!assigned)
        {
            Debug.Log("[FLOW] Cannot afford card — cancelling");
            CancelSelection();
            return;
        }
        ArrowManager.Instance?.AddPlannedArrow(slot);
        slot.ui?.SetSelected(false);
        slot.ui?.Refresh();
        CharacterUnit user = selectedUser;
        ClearSelection();
        RefreshHandIfSelected(user);
        CombatInfoBar.Instance?.ShowSlotInfo(slot);
        Debug.Log($"[FLOW] {user.unitName} to slot {slot.value} to {target.unitName}");
    }

    //drag drop confirm target
    public void ConfirmTargetOnSlot(Card card, CharacterUnit user, SpeedSlot slot)
    {
        if (!inputEnabled) return;
        if (card == null || user == null || slot == null) return;
        if (slot.state == SlotState.Committed ||
            slot.state == SlotState.Executed)  return;
        CharacterUnit target = TargetSelector.Instance?.GetTarget();
        if (target == null || target.IsDead) return;
        //check return value
        bool ok = ActionPlanner.AssignToSlot(user, slot, card, target);
        if (!ok) return;
        ArrowManager.Instance?.AddPlannedArrow(slot);
        slot.ui?.Refresh();
        ClearSelection();
        Debug.Log($"[FLOW] {user.unitName} → slot {slot.value} → {target.unitName} (drop)");
    }
    //cancel
    public void CancelSelection()
    {
        selectedSlot?.ui?.SetSelected(false);
        ArrowManager.Instance?.HidePreview();
        ClearSelection();
    }

    public void EndTargeting() => CancelSelection();

    //confirm planning (spacebar)
    public void ConfirmPlanning()
    {
        if (!inputEnabled) return;
        inputEnabled = false;
        selectedSlot?.ui?.SetSelected(false);
        ArrowManager.Instance?.HidePreview();
        ClearSelection();
        CinematicModeController.Instance?.EnterCinematic();
        StartCoroutine(RunResolve());
    }

    //turn resolve loop
    IEnumerator RunResolve()
    {
        //brief preview so player sees all arrows before they're cleared
        yield return new WaitForSeconds(1.2f);
        ArrowManager.Instance?.ClearAllArrows();
        Debug.Log("[FLOW] Handing off to pipeline");
        yield return CombatPipeline.Instance.ResolveTurn();
        //tell state machine this turn is done — TurnLoop resumes
        CombatStateMachine.Instance?.NotifyTurnComplete();
    }

    //input gate
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;

        if (!enabled)
        {
            CancelSelection();
            ArrowManager.Instance?.ClearAllArrows();
        }

        Debug.Log($"[FLOW] Input: {enabled}");
    }

    //auto assign (q key)
    public void AutoAssignPlayerActions()
    {
        var players = UnitRegistry.Instance.players;
        var enemies = UnitRegistry.Instance.enemies;

        if (players == null || enemies == null) return;

        foreach (var player in players)
        {
            if (player?.deck == null) continue;

            List<Card> hand = player.deck.GetHand();
            if (hand == null || hand.Count == 0) continue;

            int allowedSlots  = player.GetSpeedDiceCount();
            int slotsAssigned = 0;  //renamed — was conflicting with bool 'assigned'

            foreach (var slot in player.speedSlots)
            {
                if (slotsAssigned >= allowedSlots) break;

                if (slot.state == SlotState.Committed ||
                    slot.state == SlotState.Executed  ||
                    slot.state == SlotState.Planned)
                {
                    slotsAssigned++;
                    continue;
                }

                if (hand.Count == 0) break;

                Card card = hand[Random.Range(0, hand.Count)];
                CharacterUnit target = enemies.Count > 0
                    ? enemies[Random.Range(0, enemies.Count)]
                    : null;

                if (target == null || target.IsDead) continue;

                bool couldAssign = ActionPlanner.AssignToSlot(player, slot, card, target);
                if (!couldAssign) break; //out of light

                ArrowManager.Instance?.AddPlannedArrow(slot);
                slot.ui?.Refresh();
                slotsAssigned++;
            }
        }

        if (selectedUser?.deck != null)
            HandUI.Instance?.Refresh(selectedUser.deck);

        Debug.Log("[FLOW] Auto-assign complete");
    }

    //unit selection
    public void SelectUnit(CharacterUnit unit)
    {
        if (unit?.deck != null)
            HandUI.Instance?.Show(unit.deck);
    }

    public void RefreshHandIfSelected(CharacterUnit unit)
    {
        if (unit?.deck == null) return;
        HandUI.Instance?.Refresh(unit.deck);
    }

    //helpers sigma
    public void ResetAll()
    {
        CancelSelection();
        ArrowManager.Instance?.ClearAllArrows();
    }

    void ClearSelection()
    {
        selectedCard = null;
        selectedUser = null;
        selectedSlot = null;
    }

    //keyboard shortcuts
    void Update()
    {
        if (!inputEnabled) return;

        //live preview arrow while a card is selected
        if (IsTargeting && selectedUser != null)
        {
            Vector3 from = selectedUser.clashAnchor != null
                ? selectedUser.clashAnchor.position
                : selectedUser.transform.position;

            Vector3 to = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            to.z = 0f;

            ArrowManager.Instance?.UpdatePreview(from, to);
        }
        else
        {
            ArrowManager.Instance?.HidePreview();
        }

        if (Input.GetKeyDown(KeyCode.Q))     AutoAssignPlayerActions();
        if (Input.GetKeyDown(KeyCode.Space)) ConfirmPlanning();
        if (Input.GetKeyDown(KeyCode.R))     CancelSelection();
    }
}