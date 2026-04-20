using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    public void PlayCard(CardData cardData)
    {
        Debug.Log("PLAY CARD: " + cardData.Name);
        if (cardData == null || cardData.Effects == null)
            return;

        foreach (CardEffect effect in  cardData.Effects)
        {
            Debug.Log("Effect: " + effect.type + " Value: " + effect.value);
            switch (effect.type)
            {
                case EffectType.Damage:
                    enemy.TakeDamage(effect.value);
                    break;

                case EffectType.Heal:
                    GameManager.gameManager._playerHealth.healing(effect.value);
                    break;

                case EffectType.Block:
                    // TODO: add block system later
                    break;
            }
        }
    }
}
