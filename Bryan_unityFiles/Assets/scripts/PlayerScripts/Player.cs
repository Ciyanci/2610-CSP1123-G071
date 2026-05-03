using UnityEngine;
using System.Collections.Generic;


public class Player : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    private List<StatusEffect> effects = new();

    public void PlayCard(CardData cardData)
    {
        //if cannot find card data or its effects will return nothing
        if (cardData == null || cardData.Effects == null)
            return;

        foreach (CardEffect effect in  cardData.Effects)
        {
            Debug.Log("Effect: " + effect.type + " Value: " + effect.value);
            //executes code block on a case by case basis
            switch (effect.type)
            {
                case EffectType.Damage:
                    int finalDamage = ModifyDamage(effect.value);
                    enemy.TakeDamage(finalDamage);
                    break;

                case EffectType.Heal:
                //not really working???
                    GameManager.gameManager._playerHealth.healing(effect.value);
                    break;

                case EffectType.Block:
                    // add block system later
                    break;
                
                case EffectType.ApplyDamageUp:
                    AddEffect(StatusEffectType.DamageUp, effect.value);
                    break;

                case EffectType.ApplyDamageDown:
                    AddEffect(StatusEffectType.DamageDown, effect.value);
                    break;

                case EffectType.ApplyResistanceUp:
                    enemy.AddEffect(StatusEffectType.ResistanceUp, effect.value);
                    break;

                case EffectType.ApplyResistanceDown:
                    enemy.AddEffect(StatusEffectType.ResistanceDown, effect.value);
                    break;
            }
        }
    }

    public int ModifyDamage(int baseDamage)
    {
        float final = baseDamage;
        foreach(StatusEffect effect in effects)
        {
            switch (effect.type)
            {
                case StatusEffectType.DamageUp:
                    final *= (1f + 0.1f * effect.stacks);
                    break;

                case StatusEffectType.DamageDown:
                    final *= (1f - 0.1f * effect.stacks);
                    break;
            }
        }
        return Mathf.RoundToInt(final);
    }

    public void AddEffect(StatusEffectType type, int stacks)
    {
        StatusEffect existing = effects.Find(effects => effects.type == type);
        if (existing != null)
        {
            existing.stacks += stacks;
        }
        else
        {
            effects.Add(new StatusEffect{type = type, stacks = stacks});
        }
        Debug.Log("Player ganed " + type + " x" + stacks);
    }

    public void ClearEffects()
    {
        effects.Clear();
    }
}
