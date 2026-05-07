using UnityEngine;

public class Card
{
    private readonly CardData data;
    //takes values from CardData script
    public string Title => data.Name;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public int Cost => data.Cost;
    //once its set in the constructor it cannot be modified
public CardData Data => data;

    //constructor for the card
    public Card(CardData cardData)
    {
        data = cardData;
    }

}
