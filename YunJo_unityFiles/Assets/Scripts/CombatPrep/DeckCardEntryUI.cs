using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckCardEntryUI : MonoBehaviour
{
    public Image           artwork;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public Button          button;

    CardData      boundCard;
    CharacterUnit boundUnit;

    public void Setup(CardData card, CharacterUnit unit, bool isInteractable)
    {
        boundCard = card;
        boundUnit = unit;

        if (artwork      != null && card.Artwork != null) artwork.sprite = card.Artwork;
        if (cardNameText != null) cardNameText.text = card.Name;
        if (costText     != null) costText.text     = card.Cost.ToString();

        if (button != null)
        {
            button.interactable = isInteractable;
            button.onClick.RemoveAllListeners();
            if (isInteractable)
                button.onClick.AddListener(() =>
                    CombatPrepManager.Instance?.OpenCardEditorWindow(boundUnit));
        }
    }
}
