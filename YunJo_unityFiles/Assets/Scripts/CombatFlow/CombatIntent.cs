[System.Serializable]
public class CombatIntent
{
    public CharacterUnit user;
    public CharacterUnit target;

    public SpeedSlot speedSlot;
    public Card card;

    public int priority;

    public CombatPageRuntime CreatePage()
    {
        return new CombatPageRuntime(user, target, card);
    }
}