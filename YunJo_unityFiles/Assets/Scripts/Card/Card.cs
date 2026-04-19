using UnityEngine;

[System.Serializable]
public class Card
{
    public string name;
    public int min;
    public int max;
    public int damage;
    public CardData data; //bryan's system ask him if you screw up

    public string Title => data != null ? data.name : name;
    public string Description => data != null ? data.Description : "";
    public Sprite Image => data != null ? data.Artwork : null;
    public int Cost => data != null ? data.Cost : 0;
}