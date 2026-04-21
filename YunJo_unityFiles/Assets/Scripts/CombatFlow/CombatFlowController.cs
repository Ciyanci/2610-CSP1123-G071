using UnityEngine;

public class CombatFlowController : MonoBehaviour
{
    public static CombatFlowController Instance;

    public CharacterUnit selectedUnit;
    public Card selectedCard;

    public BattleFlowController battleFlow;
    public ArrowController arrow;

    public bool inputEnabled;

    void Awake()
    {
        Instance = this;
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;

        if (!enabled)
            ResetAll();
    }

    public void RefreshIntentPreview()
    {
        //reactivate arrows if intents already exist
        if (selectedUnit != null && selectedCard != null)
        {
            arrow.Begin(selectedUnit, selectedCard);
        }
    }

    public void SelectUnit(CharacterUnit unit)
    {
        if (!inputEnabled) return;

        selectedUnit = unit;
        selectedCard = null;

        unit.GetComponentInChildren<CardDeck>()?.OpenDeck();
    }

    public void SelectCard(Card card, CharacterUnit user)
    {
        if (!inputEnabled) return;

        selectedUnit = user;
        selectedCard = card;

        arrow.Begin(user, card);
    }

    public void ConfirmTarget(CharacterUnit target)
    {
        if (!inputEnabled) return;
        if (selectedCard == null || selectedUnit == null) return;

        battleFlow.QueueAction(selectedUnit, target, selectedCard);

        arrow.End();
        selectedCard = null;
    }

    public void Cancel()
    {
        arrow.End();
        selectedUnit = null;
        selectedCard = null;
    }

    public void ResetAll()
    {
        Cancel();
    }
}