using UnityEngine;

[System.Serializable]
public class PreviewIntent
{
    public CharacterUnit user;
    public CharacterUnit target;
    public Card card;

    // purely visual
    public ArrowController arrow;
}