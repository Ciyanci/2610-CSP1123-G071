using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Combat/Card")]
public class CardData : ScriptableObject
{
    [Header("Info")]
    public string Name;

    [TextArea]
    public string Description;

    [TextArea]
    public string Effects;

    public Sprite Artwork;

    [Header("Combat")]
    public int Cost;

    [Header("Dice")]
    public List<DiceData> dice = new();

    [Header("Rarity")]
    public CardRarity rarity;
}

public enum CardRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}