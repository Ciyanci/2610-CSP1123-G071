using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatFlowController : MonoBehaviour
{
    public static CombatFlowController Instance;

    [Header("State")]
    public bool inputEnabled;

    // Selection state — three steps: card → slot → target
    Card selectedCard;
    CharacterUnit selectedUser;
    SpeedSlot selectedSlot;         // ← new: the specific slot the player clicked

    [Header("Visuals")]
    public ArrowController arrow;

    public bool IsTargeting => selectedCard != null;

    void Awake()
    {
        Instance = this;
    }

    // =========================
    // STEP 1 — select a card
    // Called by CardView.OnPointerDown
    // =========================
    public void StartTargeting(Card card, CharacterUnit user)
    {
        if (!inputEnabled || card == null) return;

        selectedCard = card;
        selectedUser = user;
        selectedSlot = null;        // clear any previous slot selection

        RectTransform ui = HandUI.Instance?.GetCardUI(user, card);
        arrow?.Begin(ui);

        Debug.Log($"[FLOW] Card selected: {card.Name} — now pick a speed slot");
    }

    // =========================
    // STEP 2 — select a specific speed slot
    // Called by SpeedSlotUIElement when clicked
    // =========================
    public void SelectSlot(SpeedSlot slot)
    {
        if (!inputEnabled) return;
        if (selectedCard == null) return;       // must have a card first
        if (slot == null || slot.owner == null) return;

        // Only allow selecting slots owned by players
        if (!UnitRegistry.Instance.players.Contains(slot.owner)) return;

        // Slot must be available
        if (slot.state == SlotState.Committed ||
            slot.state == SlotState.Executed) return;

        // If slot already has a card, unassign it first (swap behaviour)
        if (slot.state == SlotState.Planned && slot.assignedCard != null)
        {
            slot.owner.deck?.ReturnToHand(slot.assignedCard);
            slot.Clear();
        }

        selectedSlot = slot;

        // Highlight the selected slot
        slot.ui?.SetSelected(true);

        Debug.Log($"[FLOW] Slot selected: value {slot.value} — now pick a target");
    }

    // =========================
    // STEP 3 — confirm target
    // Called by CharacterUnit.OnMouseDown when IsTargeting is true
    // =========================
    public void ConfirmTarget(CharacterUnit target)
    {
        if (selectedCard == null || selectedUser == null) return;
        if (target == null || target.IsDead) return;

        // If no slot was manually picked, fall back to highest available
        SpeedSlot slot = selectedSlot ?? selectedUser.GetHighestAvailableSlot();

        if (slot == null)
        {
            Debug.Log("[FLOW] No available slot");
            EndTargeting();
            return;
        }

        ActionPlanner.AssignToSlot(selectedUser, slot, selectedCard, target);

        // Preview state on this slot
        SpeedSlot enemySlot = FindSlotTargeting(target);
        TargetPreviewState preview = enemySlot != null
            ? TargetPreviewState.WillClash
            : TargetPreviewState.Unopposed;

        slot.ui?.SetSelected(false);
        slot.ui?.ShowPreview(preview);

        arrow?.End();
        ClearSelection();

        RefreshHandIfSelected(selectedUser ?? slot.owner);
    }

    // =========================
    // CONFIRM ON A SPECIFIC SLOT (drag-drop path)
    // Called by SpeedSlotUIElement.OnDrop
    // =========================
    public void ConfirmTargetOnSlot(Card card, CharacterUnit user, SpeedSlot slot)
    {
        if (!inputEnabled) return;
        if (card == null || user == null || slot == null) return;
        if (slot.state == SlotState.Committed ||
            slot.state == SlotState.Executed) return;

        CharacterUnit target = TargetSelector.Instance?.GetTarget();
        if (target == null || target.IsDead) return;

        ActionPlanner.AssignToSlot(user, slot, card, target);

        SpeedSlot enemySlot = FindSlotTargeting(target);
        TargetPreviewState preview = enemySlot != null
            ? TargetPreviewState.WillClash
            : TargetPreviewState.Unopposed;

        slot.ui?.ShowPreview(preview);

        arrow?.End();
        ClearSelection();

        Debug.Log($"[FLOW] {user.unitName} → {slot.value} die → {target.unitName} ({preview})");
    }

    // =========================
    // END TARGETING (cancel)
    // =========================
    public void EndTargeting()
    {
        if (selectedSlot != null)
            selectedSlot.ui?.SetSelected(false);

        arrow?.End();
        ClearSelection();
    }

    // =========================
    // CONFIRM PLANNING
    // =========================
    public void ConfirmPlanning()
    {
        if (!inputEnabled) return;

        inputEnabled = false;
        StartCoroutine(CombatPipeline.Instance.ResolveTurn());

        Debug.Log("[FLOW] Planning locked → resolving combat");
    }

    // =========================
    // HELPERS
    // =========================
    public void SelectUnit(CharacterUnit unit)
    {
        if (unit?.deck != null)
            HandUI.Instance.Show(unit.deck);
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;

        if (!enabled)
            EndTargeting();
    }

    public void RefreshHandIfSelected(CharacterUnit unit)
    {
        if (unit?.deck == null) return;
        HandUI.Instance?.Refresh(unit.deck);
    }

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
        arrow?.End();
        ClearSelection();
    }

    // =========================
    // AUTO ASSIGN (Q key)
    // =========================
    public void AutoAssignPlayerActions()
    {
        var players = UnitRegistry.Instance.players;
        var enemies = UnitRegistry.Instance.enemies;

        if (players == null || enemies == null) return;

        foreach (var player in players)
        {
            if (player == null || player.deck == null) continue;

            List<Card> hand = player.deck.GetHand();
            if (hand == null || hand.Count == 0) continue;

            // Fill ALL available slots, not just the highest
            foreach (var slot in player.speedSlots)
            {
                if (slot.state == SlotState.Committed ||
                    slot.state == SlotState.Executed  ||
                    slot.state == SlotState.Planned) continue;

                if (hand.Count == 0) break;

                Card randomCard = hand[Random.Range(0, hand.Count)];
                CharacterUnit target = enemies.Count > 0
                    ? enemies[Random.Range(0, enemies.Count)]
                    : null;

                if (target == null) continue;

                ActionPlanner.AssignToSlot(player, slot, randomCard, target);

                // Preview
                SpeedSlot enemySlot = FindSlotTargeting(target);
                slot.ui?.ShowPreview(enemySlot != null
                    ? TargetPreviewState.WillClash
                    : TargetPreviewState.Unopposed);
            }
        }

        if (selectedUser?.deck != null)
            HandUI.Instance?.Refresh(selectedUser.deck);

        Debug.Log("[FLOW] Auto-assignment complete");
    }

    void Update()
    {
        if (!inputEnabled) return;

        if (Input.GetKeyDown(KeyCode.Q))
            AutoAssignPlayerActions();

        if (Input.GetKeyDown(KeyCode.Space))
            ConfirmPlanning();

        // Escape cancels current selection
        if (Input.GetKeyDown(KeyCode.Escape))
            EndTargeting();
    }
}