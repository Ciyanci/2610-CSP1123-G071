using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckCardEntryUI : MonoBehaviour
{
    public Image           artwork;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public Button          button;  //click to open card editor (player) or nothing (enemy)

    CardData       boundCard;
    TeamRosterSlot boundSlot;

    public void Setup(CardData card, TeamRosterSlot slot, bool isInteractable)
    {
        boundCard = card;
        boundSlot = slot;

        if (artwork      != null && card.Artwork != null) artwork.sprite = card.Artwork;
        if (cardNameText != null) cardNameText.text = card.Name;
        if (costText     != null) costText.text     = card.Cost.ToString();

        button?.onClick.RemoveAllListeners();

        if (isInteractable && button != null)
        {
            button.interactable = true;
            button.onClick.AddListener(() =>
                CombatPrepManager.Instance?.OpenCardEditorWindow(boundSlot));
        }
        else if (button != null)
        {
            button.interactable = false;
        }
    }
}
