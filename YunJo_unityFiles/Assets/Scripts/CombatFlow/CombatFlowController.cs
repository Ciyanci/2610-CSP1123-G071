using UnityEngine;

public class CombatFlowController : MonoBehaviour
{
    public static CombatFlowController Instance;

    [Header("State")]
    public bool inputEnabled;

    public Card selectedCard;
    public CharacterUnit selectedUser;
    public CharacterUnit selectedUnit;

    [Header("Visuals")]
    public ArrowController arrow;

    void Awake()
    {
        Instance = this;
    }

    // =========================
    // INPUT CONTROL
    // =========================

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
        Debug.Log($"[FLOW] Input Enabled: {enabled}");
    }

    // =========================
    // UNIT SELECTION (CLICK CHARACTER)
    // =========================

    public void SelectUnit(CharacterUnit unit)
    {
        selectedUnit = unit;

        Debug.Log($"[FLOW] Selected unit: {unit.name}");

        if (unit.deck != null)
        {
            HandUI.Instance.Show(unit.deck);
        }
    }
    // =========================
    // CARD SELECTION
    // =========================

    public void SelectCard(Card card, CharacterUnit user)
    {
        if (!inputEnabled || card == null || user == null)
            return;

        selectedCard = card;
        selectedUser = user;

        Debug.Log($"[SELECT] {user.name} picked {card.Data.Name}");

        // start arrow preview from user
        if (arrow != null)
        {
            arrow.Begin(user, card);
        }
    }

    // (kept for compatibility with your other scripts)
    public void StartTargeting(Card card, CharacterUnit user)
    {
        SelectCard(card, user);
    }

    // =========================
    // TARGET CONFIRMATION (CLICK ENEMY)
    // =========================

    public void ConfirmTarget(CharacterUnit target)
    {
        if (!inputEnabled || selectedCard == null || selectedUser == null)
            return;

        if (target == null || target == selectedUser)
            return;

        var flow = BattleFlowController.Instance;

        flow.QueuePreview(selectedUser, target, selectedCard);

        Debug.Log($"[INTENT] {selectedUser.name} → {target.name}");

        // consume card from deck
        var deck = selectedUser.GetComponent<CharacterDeck>();
        if (deck != null)
            deck.UseCard(selectedCard);

        // refresh UI
        HandUI.Instance.Refresh(selectedUnit.deck);

        // end arrow
        if (arrow != null)
            arrow.End();

        ClearSelection();
    }

    // =========================
    // RESET
    // =========================

    public void ClearSelection()
    {
        selectedCard = null;
        selectedUser = null;
    }

    public void ResetAll()
    {
        if (arrow != null)
            arrow.End();

        ClearSelection();
    }
}