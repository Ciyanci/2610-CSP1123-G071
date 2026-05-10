using UnityEngine;

[System.Serializable]
public class PreviewIntent
{
    public CharacterUnit user;
    public CharacterUnit target;
    public Card card;
    public SpeedSlot slot;      // 🔥 CRITICAL ADDITION
    public ArrowController arrow;
}