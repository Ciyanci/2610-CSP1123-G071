using System.Collections.Generic;

public static class PageBuilder
{
    public static CombatPageRuntime Build(CombatIntent intent)
    {
        return new CombatPageRuntime(
            intent.user,
            intent.target,
            intent.card
        );
    }
}