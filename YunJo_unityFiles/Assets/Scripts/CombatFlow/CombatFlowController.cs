using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatFlowController : MonoBehaviour
{
    public static CombatFlowController Instance;

    [Header("State")]
    public bool inputEnabled;

    //3 steps : card then slot then target (easy enough right)
    Card          selectedCard;
    CharacterUnit selectedUser;
    SpeedSlot     selectedSlot;

    public bool IsTargeting => selectedCard != null;

    void Awake()
    {
        Instance = this;
    }

    //step 1 (selecting card)
    public void StartTargeting(Card card, CharacterUnit user)
    {
        if (!inputEnabled || card == null) return;

        //cancel previously selected cards
        CancelSelection();

        selectedCard = card;
        selectedUser = user;
        selectedSlot = null;

        Debug.Log($"[FLOW] Card selected: {card.Name} — pick a speed slot");
    }

    //step 2 (select speed sot)
    public void SelectSlot(SpeedSlot slot)
    {
        if (!inputEnabled)    return;
        if (selectedCard == null) return;
        if (slot?.owner == null)  return;

        //players only able to do this
        if (!UnitRegistry.Instance.players.Contains(slot.owner)) return;

        if (slot.state == SlotState.Committed ||
            slot.state == SlotState.Executed)  return;

        //swap card (returns existing card to hand)
        if (slot.state == SlotState.Planned && slot.assignedCard != null)
        {
            slot.owner.deck?.ReturnToHand(slot.assignedCard);
            ArrowManager.Instance?.RemovePlannedArrow(slot);
            slot.Clear();
        }

        //deselect previous slot highlight
        selectedSlot?.ui?.SetSelected(false);

        selectedSlot = slot;
        slot.ui?.SetSelected(true);

        Debug.Log($"[FLOW] Slot selected: value {slot.value} — pick a target");
    }

    //step 3 (confirm target)
    public void ConfirmTarget(CharacterUnit target)
    {
        if (selectedCard == null || selectedUser == null) return;
        if (target == null || target.IsDead)              return;

        SpeedSlot slot = selectedSlot ?? selectedUser.GetHighestAvailableSlot();

        if (slot == null)
        {
            Debug.Log("[FLOW] No available slot");
            CancelSelection();
            return;
        }

        ActionPlanner.AssignToSlot(selectedUser, slot, selectedCard, target);
        ArrowManager.Instance?.AddPlannedArrow(slot);

        slot.ui?.SetSelected(false);
        slot.ui?.Refresh();

        CharacterUnit user = selectedUser;
        ClearSelection();

        RefreshHandIfSelected(user);
        CombatInfoBar.Instance?.ShowSlotInfo(slot);

        Debug.Log($"[FLOW] {user.unitName} → slot {slot.value} → {target.unitName}");
    }

    //confirm on slot (idk why dragging doesnt work gotta work on this **)
    public void ConfirmTargetOnSlot(Card card, CharacterUnit user, SpeedSlot slot)
    {
        if (!inputEnabled) return;
        if (card == null || user == null || slot == null) return;
        if (slot.state == SlotState.Committed ||
            slot.state == SlotState.Executed)  return;

        CharacterUnit target = TargetSelector.Instance?.GetTarget();
        if (target == null || target.IsDead) return;

        ActionPlanner.AssignToSlot(user, slot, card, target);
        ArrowManager.Instance?.AddPlannedArrow(slot);

        slot.ui?.Refresh();
        ClearSelection();

        Debug.Log($"[FLOW] {user.unitName} → slot {slot.value} → {target.unitName} (drop)");
    }

    //cancel (escape key)
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
        CancelSelection();

        CinematicModeController.Instance?.EnterCinematic();
        StartCoroutine(CombatPipeline.Instance.ResolveTurn());

        Debug.Log("[FLOW] Planning confirmed → resolving");
    }

    //input gatekeeper
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

    //auto assign (q) **should stop assigning on non-existent slots now hehe
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

            //only assigns to allowed slots (no more non-existent slots being assigned automatically)
            int allowedSlots = player.GetSpeedDiceCount();
            int assigned = 0;

            foreach (var slot in player.speedSlots)
            {
                if (assigned >= allowedSlots) break; //hard cap

                if (slot.state == SlotState.Committed ||
                    slot.state == SlotState.Executed  ||
                    slot.state == SlotState.Planned)
                {
                    assigned++;
                    continue;
                }

                if (hand.Count == 0) break;

                Card card = hand[Random.Range(0, hand.Count)];
                CharacterUnit target = enemies.Count > 0
                    ? enemies[Random.Range(0, enemies.Count)]
                    : null;

                if (target == null) continue;

                ActionPlanner.AssignToSlot(player, slot, card, target);
                ArrowManager.Instance?.AddPlannedArrow(slot);
                slot.ui?.Refresh();
                assigned++;
            }
        }

        if (selectedUser?.deck != null)
            HandUI.Instance?.Refresh(selectedUser.deck);

        Debug.Log("[FLOW] Auto-assign complete");
    }

    //unit slot selection info
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

    //cool helpers
    SpeedSlot FindSlotTargeting(CharacterUnit target)
    {
        foreach (var unit in UnitRegistry.Instance.players)
            foreach (var slot in unit.speedSlots)
                if (slot.target == target && slot.state == SlotState.Planned)
                    return slot;

        foreach (var unit in UnitRegistry.Instance.enemies)
            foreach (var slot in unit.speedSlots)
                if (slot.target == target && slot.state == SlotState.Planned)
                    return slot;

        return null;
    }

    void ClearSelection()
    {
        selectedCard = null;
        selectedUser = null;
        selectedSlot = null;
    }

    public void ResetAll()
    {
        CancelSelection();
        ArrowManager.Instance?.ClearAllArrows();
    }

    //update
    void Update()
    {
        if (!inputEnabled) return;

        //live preview arrow tracks cursor while a card is selected
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

        if (Input.GetKeyDown(KeyCode.Q))      AutoAssignPlayerActions();
        if (Input.GetKeyDown(KeyCode.Space))  ConfirmPlanning();
        if (Input.GetKeyDown(KeyCode.Escape)) CancelSelection();
    }
}