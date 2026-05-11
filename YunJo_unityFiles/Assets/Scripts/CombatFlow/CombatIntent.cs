[System.Serializable]
public class CombatIntent
{
    public CharacterUnit user;
    public CharacterUnit target;

    public SpeedSlot speedSlot;
    public Card card;

    public int priority;

    // ✔ NOW RETURNS RUNTIME PAGE (correct system)
    public CombatPageRuntime CreatePage()
    {
        return PageBuilder.Build(this);
    }

    public bool IsValid =>
        user != null &&
        target != null &&
        card != null;
}