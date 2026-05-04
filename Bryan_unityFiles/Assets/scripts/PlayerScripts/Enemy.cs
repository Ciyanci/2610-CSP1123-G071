using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyResistanceProfile resistanceProfile;
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
        float typeMultiplier = 1f;
        if(resistanceProfile != null)
        {
            typeMultiplier = resistanceProfile.GetMultiplier(damageType);
        }
        float finalDamage = amount * multiplier * typeMultiplier;

        finalDamage = Mathf.Max(1f, finalDamage);

        int dmg = Mathf.RoundToInt(finalDamage);
        health -= dmg;
        Debug.Log($"Enemy took {dmg} {damageType} damage (type: x{typeMultiplier}) HP left: {health}");

        if (health <= 0)
        {
            Die();
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
