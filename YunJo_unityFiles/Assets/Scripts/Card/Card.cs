using UnityEngine;
using System.Collections.Generic;

public class Card
{
    public CardData Data { get; private set; }

    List<CombatDice> runtimeDice = new();

    public int Cost => Data.Cost;
    public string Name => Data.Name;
    public Sprite Artwork => Data.Artwork;

    public Card(CardData data)
    {
        Data = data;

        foreach (var d in data.dice)
        {
            runtimeDice.Add(new CombatDice(d));
        }
    }

    public List<CombatDice> GetDice() => runtimeDice;

    public CombatDice GetDice(int index)
    {
        if (index < 0 || index >= runtimeDice.Count)
            return null;

        return runtimeDice[index];
    }

    public int DiceCount => runtimeDice.Count;
}