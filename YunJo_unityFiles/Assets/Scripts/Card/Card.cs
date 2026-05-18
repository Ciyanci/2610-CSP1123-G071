using UnityEngine;
using System.Collections.Generic;

public class Card
{
    public CardData Data { get; private set; }

    public int Cost => Data.Cost;
    public string Name => Data.Name;
    public Sprite Artwork => Data.Artwork;

    public Card(CardData data)
    {
        Data = data;
    }

    //this is where it gets the data
    public List<DiceData> GetDice()
    {
        return Data.dice;
    }

    public int DiceCount => Data.dice.Count;

    public DiceData GetDiceSafe(int index)
    {
        if (index < 0 || index >= Data.dice.Count)
            return null;

        return Data.dice[index];
    }
}