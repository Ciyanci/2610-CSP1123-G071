using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoBarDiceRow : MonoBehaviour
{
    public Image typeIcon;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI effectText;
    public Image background;

    [Header("Type Sprites")]
    public Sprite slashSprite;
    public Sprite pierceSprite;
    public Sprite bluntSprite;

    [Header("Behaviour Colors")]
    public Color attackColor = new Color(0.75f, 0.15f, 0.15f, 1f);
    public Color defendColor = new Color(0.15f, 0.35f, 0.75f, 1f);
    public Color evadeColor = new Color(0.15f, 0.65f, 0.35f, 1f);
    public Color buffColor = new Color(0.65f, 0.55f, 0.15f, 1f);

    public void Setup(DiceData data)
    {
        if (rangeText  != null) rangeText.text  = $"{data.minRoll}-{data.maxRoll}+{data.power}";
        if (effectText != null) effectText.text = data.effect.ToString();

        if (typeIcon != null)
            typeIcon.sprite = data.damageType switch
            {
                DamageType.Slash  => slashSprite,
                DamageType.Pierce => pierceSprite,
                DamageType.Blunt  => bluntSprite,
                _ => null
            };

        if (background != null)
            background.color = data.effect switch
            {
                DiceBehaviour.Attack => attackColor,
                DiceBehaviour.Defend => defendColor,
                DiceBehaviour.Evade  => evadeColor,
                DiceBehaviour.Buff   => buffColor,
                _ => attackColor
            };
    }
}
