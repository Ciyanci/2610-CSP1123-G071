public static class CombatIntentUtility
{
    public static bool IsValid(CombatIntent intent)
    {
        if (intent == null)
            return false;

        if (intent.user == null || intent.target == null)
            return false;

        if (intent.user.IsDead)
            return false;

        if (intent.target.IsDead)
            return false;

        if (!intent.user.CanAct)
            return false;

        return true;
    }
}