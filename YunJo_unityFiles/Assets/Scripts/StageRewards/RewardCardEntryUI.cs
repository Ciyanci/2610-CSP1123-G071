using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RewardCardEntryUI : MonoBehaviour
{
    public Image           artwork;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public Button          selectButton;

    CardData boundCard;
    Action<CardData> onSelected;

    public void Setup(CardData card, Action<CardData> callback)
    {
        boundCard  = card;
        onSelected = callback;

        if (artwork      != null && card.Artwork != null)
            artwork.sprite = card.Artwork;
        if (cardNameText != null) cardNameText.text = card.Name;
        if (costText     != null) costText.text     = card.Cost.ToString();

        selectButton?.onClick.RemoveAllListeners();
        selectButton?.onClick.AddListener(() => onSelected?.Invoke(boundCard));
    }
}
