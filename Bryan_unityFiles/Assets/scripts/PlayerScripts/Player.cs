using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    public void PlayCard(CardData cardData)
    {
        Debug.Log("PLAY CARD: " + cardData.Name);
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
                    enemy.TakeDamage(effect.value);
                    break;

                case EffectType.Heal:
                //not really working???
                    GameManager.gameManager._playerHealth.healing(effect.value);
                    break;

                case EffectType.Block:
                    // add block system later
                    break;
            }
        }
    }
}
