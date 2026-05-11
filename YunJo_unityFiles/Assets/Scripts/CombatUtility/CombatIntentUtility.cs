public static class CombatIntentUtility
{
    public static bool IsValid(CombatIntent intent)
    {
        return intent != null && intent.IsValid;
    }
}