using UnityEngine;

public class CombatFlowController : MonoBehaviour
{
    public static CombatFlowController Instance;

    public bool inputEnabled;

    CharacterUnit selectedUnit;
    Card selectedCard;

    public ArrowController arrow;

    void Awake()
    {
        Instance = this;
    }

    public void SetInputEnabled(bool value)
    {
        inputEnabled = value;
    }

    public void SelectUnit(CharacterUnit unit)
    {
        selectedUnit = unit;
        Debug.Log($"[FLOW] Selected unit: {unit.name}");

        var handUI = FindFirstObjectByType<HandUI>();
        handUI.Show(unit.deck);
    }

    public void SelectCard(Card card, CharacterUnit user)
    {
        if (!inputEnabled) return;

        selectedUnit = user;
        selectedCard = card;
        Debug.Log($"[FLOW] Selected card: {card.Data.Name} by {selectedUnit.name}");

        arrow.Begin(user, card);
    }

    public void ConfirmTarget(CharacterUnit target)
    {
        if (!inputEnabled || selectedCard == null) return;

        var flow = FindFirstObjectByType<BattleFlowController>();

        flow.QueuePreview(selectedUnit, target, selectedCard);
        Debug.Log($"[FLOW] Target selected: {target.name}");
        Debug.Log($"[ARROW] Attempting target selection");

        arrow.End();
        selectedCard = null;
    }

    public void ResetAll()
    {
        arrow.End();
        selectedCard = null;
    }
}