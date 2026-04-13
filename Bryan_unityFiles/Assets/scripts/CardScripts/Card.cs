using UnityEngine;

public class Card
{
    //takes values from CardData script
    public string Title => data.name;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public int Cost {get; private set;}

    //once its set in the constructor it cannot be modified
    private readonly CardData data;

    //constructor for the card
    public Card(CardData cardData)
    {
        data = cardData;
        Cost = cardData.Cost;
    }

}
