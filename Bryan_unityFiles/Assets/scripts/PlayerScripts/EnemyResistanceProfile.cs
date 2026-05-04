using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DamageTypeModifier
{
    public DamageType type;
    public float multiplier;
}

public class EnemyResistanceProfile : MonoBehaviour
{
    [SerializeField] private List<DamageTypeModifier> modifiers = new();

    public float GetMultiplier(DamageType type)
    {
        foreach (var mod in modifiers)
        {
            if (mod.type == type)
                return mod.multiplier;
        }
        return 1f;
    }
}
