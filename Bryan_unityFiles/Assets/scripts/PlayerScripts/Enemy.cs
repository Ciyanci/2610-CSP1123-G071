using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public float health = 100;

    private List<StatusEffect> effects = new();


    public void TakeDamage(int amount, DamageType damageType)
    {
        float reduction = 0f;
        float vulnerability = 0f;

        foreach(var effect in effects)
        {
            switch (effect.type)
            {
                case StatusEffectType.ResistanceUp:
                    reduction += effect.stacks * 0.1f;
                    break;

                case StatusEffectType.ResistanceDown:
                    vulnerability += effect.stacks * 0.1f;
                    break;
            }
        }

        float multiplier = Mathf.Clamp01(1f - reduction + vulnerability);
        float typeMultiplier = GetTypeMultiplier(damageType);
        float finalDamage = amount * multiplier * typeMultiplier;

        finalDamage = Mathf.Max(1f, finalDamage);

        int dmg = Mathf.RoundToInt(finalDamage);
        health -= dmg;
        Debug.Log($"Enemy took {dmg} {damageType} damage (base: {amount}, type: x{typeMultiplier}) HP left: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    float GetTypeMultiplier(DamageType type)
    {
        switch (type)
        {
            case DamageType.Slash:
                return 1.2f;
            
            case DamageType.Pierce:
                return 0.8f;

            case DamageType.Blunt:
                return 1f;

            default:
                return 1f;
        }
    }

    public void AddEffect(StatusEffectType type, int stacks)
    {
        var existing = effects.Find(effects => effects.type == type);

        if (existing != null)
        {
            existing.stacks += stacks;
        }
        else
        {
            effects.Add(new StatusEffect {type = type, stacks = stacks});
        }
        Debug.Log($"Enemy gained {type} x{stacks}");
    }

    public void ClearEffects()
    {
        effects.Clear();
    }

    void Die()
    {
        Debug.Log("Enemy Defeated!");
    }
}
