using UnityEngine;

public static class DamageCalculator
{
    public static int Calculate(
        int raw,
        DamageType type,
        CharacterUnit target)
    {
        float resist =
            target.resistances.GetModifier(type);

        return Mathf.Max(
            1,
            Mathf.RoundToInt(raw * resist)
        );
    }
}