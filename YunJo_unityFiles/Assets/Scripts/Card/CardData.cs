using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Card")]
public class CardData : ScriptableObject
{
    public string Name;
    public string Description;

    public Sprite Image;
    public Sprite Artwork;

    public int Cost;

    [Header("Dice")]
    public int MinRoll;
    public int MaxRoll;

    [Header("Damage")]
    public int Damage;

    public CardView prefab;
}