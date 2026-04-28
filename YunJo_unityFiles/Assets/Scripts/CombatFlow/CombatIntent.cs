using UnityEngine;

[System.Serializable]
public class CombatIntent
{
    public CharacterUnit user;
    public CharacterUnit target;
    public Card card;

    public bool resolved;
}