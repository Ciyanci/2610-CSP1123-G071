using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CardInfoPanel : MonoBehaviour
{
    public static CardInfoPanel Instance; // singleton so cardview can call it directly

    [Header ("Refrences")] // displays everything
    public GameObject panel;
    public Image background;
    public TMP_Text title;
    public TMP_Text cost;
    public TMP_Text description;
    public Image artwork;
    public TMP_Text rarityLabel;
    public TMP_Text effects;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(Card card)
    {
        panel.SetActive(true);
        title.text = card.Data.Name;
        cost.text = card.Cost.ToString();
        description.text = card.Data.Description;
        artwork.sprite = card.Artwork;
        effects.text = card.Data.Effects;

        background.color = card.Data.rarity switch
        {
            CardRarity.Common => Color.white,
            CardRarity.Uncommon => Color.green,
            CardRarity.Rare => Color.cyan,
            CardRarity.Epic => new Color (0.7f, 0.2f, 1f),
            CardRarity.Legendary => Color.yellow,
        };

        rarityLabel.text = card.Data.rarity.ToString();
    }

    public void Hide()
    {
        panel.SetActive(false);

    }
}
