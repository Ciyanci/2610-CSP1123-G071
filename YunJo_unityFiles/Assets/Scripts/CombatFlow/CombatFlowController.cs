using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatFlowController : MonoBehaviour
{
    public static CombatFlowController Instance;

    [Header("State")]
    public bool inputEnabled;

    // Three-step selection: card → slot → target
    Card          selectedCard;
    CharacterUnit selectedUser;
    SpeedSlot     selectedSlot;

    public bool IsTargeting => selectedCard != null;

    void Awake()
    {
        Instance = this;
    }

    // =========================
    // STEP 1 — select a card
    // =========================
    public void StartTargeting(Card card, CharacterUnit user)
    {
        if (!inputEnabled || card == null) return;

        // Cancel any previous selection cleanly
        CancelSelection();

        selectedCard = card;
        selectedUser = user;
        selectedSlot = null;

        Debug.Log($"[FLOW] Card selected: {card.Name} — pick a speed slot");
    }

    // =========================
    // STEP 2 — select a specific speed slot
    // =========================
    public void SelectSlot(SpeedSlot slot)
    {
        if (!inputEnabled)    return;
        if (selectedCard == null) return;
        if (slot?.owner == null)  return;

        // Players only
        if (!UnitRegistry.Instance.players.Contains(slot.owner)) return;

        if (slot.state == SlotState.Committed ||
            slot.state == SlotState.Executed)  return;

        // Swap — return existing card to hand first
        if (slot.state == SlotState.Planned && slot.assignedCard != null)
        {
            slot.owner.deck?.ReturnToHand(slot.assignedCard);
            ArrowManager.Instance?.RemovePlannedArrow(slot);
            slot.Clear();
        }

        // Deselect previous slot highlight
        selectedSlot?.ui?.SetSelected(false);

        selectedSlot = slot;
        slot.ui?.SetSelected(true);

        Debug.Log($"[FLOW] Slot selected: value {slot.value} — pick a target");
    }

    // =========================
    // STEP 3 — confirm target (click on enemy)
    // =========================
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

        Debug.Log($"[FLOW] {user.unitName} → slot {slot.value} → {target.unitName}");
    }

    // =========================
    // CONFIRM ON SLOT (drag-drop path)
    // =========================
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

    // =========================
    // CANCEL (Escape / explicit)
    // Hides preview but keeps planned arrows
    // =========================
    public void CancelSelection()
    {
        selectedSlot?.ui?.SetSelected(false);
        ArrowManager.Instance?.HidePreview();
        ClearSelection();
    }

    // Alias kept so existing call sites compile
    public void EndTargeting() => CancelSelection();

    // =========================
    // CONFIRM PLANNING (Space)
    // =========================
    public void ConfirmPlanning()
    {
        if (!inputEnabled) return;

        inputEnabled = false;
        CancelSelection();

        StartCoroutine(CombatPipeline.Instance.ResolveTurn());

        Debug.Log("[FLOW] Planning confirmed → resolving");
    }

    // =========================
    // INPUT GATE
    // =========================
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

    // =========================
    // AUTO ASSIGN (Q)
    // =========================
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

            foreach (var slot in player.speedSlots)
            {
                if (slot.state == SlotState.Committed ||
                    slot.state == SlotState.Executed  ||
                    slot.state == SlotState.Planned)   continue;

                if (hand.Count == 0) break;

                Card          card   = hand[Random.Range(0, hand.Count)];
                CharacterUnit target = enemies.Count > 0
                    ? enemies[Random.Range(0, enemies.Count)]
                    : null;

                if (target == null) continue;

                ActionPlanner.AssignToSlot(player, slot, card, target);
                ArrowManager.Instance?.AddPlannedArrow(slot);
                slot.ui?.Refresh();
            }
        }

        if (selectedUser?.deck != null)
            HandUI.Instance?.Refresh(selectedUser.deck);

        Debug.Log("[FLOW] Auto-assign complete");
    }

    // =========================
    // UNIT / SLOT SELECTION (info bar)
    // =========================
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

    // =========================
    // HELPERS
    // =========================
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

    // =========================
    // UPDATE
    // =========================
    void Update()
    {
        if (!inputEnabled) return;

        // Live preview arrow tracks cursor while a card is selected
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