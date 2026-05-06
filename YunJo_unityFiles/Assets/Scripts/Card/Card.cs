using UnityEngine;

public class Card
{
    public CardData Data { get; private set; }

    public int Cost => Data.Cost;

    public int Min => Data.MinRoll;
    public int Max => Data.MaxRoll;
    public int Damage => Data.Damage;

    // ✅ FIXED: instance access
    public Sprite Artwork => Data.Artwork;

    // 🔥 BACKWARD COMPATIBILITY
    public int min => Min;
    public int max => Max;
    public int damage => Damage;

    public Card(CardData data)
    {
        Data = data;
    }
}