using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardListEntryUI : MonoBehaviour
{
    public Image           artwork;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public Image           inDeckOverlay;
    public Button          addButton;

    CardData      boundCard;
    CharacterUnit boundUnit;

    public void Setup(CardData card, bool alreadyInDeck, CharacterUnit unit)
    {
        boundCard = card;
        boundUnit = unit;

        if (artwork      != null && card.Artwork != null) artwork.sprite = card.Artwork;
        if (cardNameText != null) cardNameText.text = card.Name;
        if (costText     != null) costText.text     = card.Cost.ToString();

        inDeckOverlay?.gameObject.SetActive(alreadyInDeck);

        addButton?.onClick.RemoveAllListeners();
        addButton?.onClick.AddListener(() =>
        {
            CombatPrepManager.Instance?.AddCard(boundUnit, boundCard);
            inDeckOverlay?.gameObject.SetActive(true);
        });
    }
}
