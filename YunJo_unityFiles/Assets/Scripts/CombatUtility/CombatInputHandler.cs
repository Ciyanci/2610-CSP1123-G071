using UnityEngine;

public class CombatInputController : MonoBehaviour
{
    public static CombatInputController Instance;

    public bool inputEnabled;

    CharacterUnit selectedUnit;
    Card selectedCard;

    public ArrowController arrow;
    public BattleFlowController battle;

    void Awake()
    {
        Instance = this;
    }

    public void SetInput(bool state)
    {
        inputEnabled = state;

        if (!state)
            Cancel();
    }

    public void SelectUnit(CharacterUnit unit)
    {
        if (!inputEnabled) return;

        selectedUnit = unit;
        unit.Highlight(true);

        unit.GetComponentInChildren<CardDeck>()?.OpenDeck();
    }

    public void SelectCard(Card card, CharacterUnit user)
    {
        if (!inputEnabled) return;

        selectedUnit = user;
        selectedCard = card;

        arrow.Begin(user, card);
    }

    public void SelectTarget(CharacterUnit target)
    {
        if (!inputEnabled) return;
        if (selectedUnit == null || selectedCard == null) return;

        battle.RegisterIntent(selectedUnit, selectedCard, target);

        arrow.End();

        selectedCard = null;
    }

    public void Cancel()
    {
        arrow.End();
        selectedUnit?.Highlight(false);

        selectedUnit = null;
        selectedCard = null;
    }
}