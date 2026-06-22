using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpandedDiceRowUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text rangeText;
    public TMP_Text effectText;
    public TMP_Text powerText;

    public Sprite slashSprite;
    public Sprite pierceSprite;
    public Sprite bluntSprite;

    [Header("Behaviour Colors")]
    public Color attackColor = new Color(0.75f, 0.15f, 0.15f, 1f);
    public Color defendColor = new Color(0.15f, 0.35f, 0.75f, 1f);
    public Color evadeColor  = new Color(0.15f, 0.65f, 0.35f, 1f);
    public Color buffColor   = new Color(0.65f, 0.55f, 0.15f, 1f);

    public Image background;

    public void Setup(DiceData data)
    {
        //range — e.g. "2 – 6"
        if (rangeText != null)
            rangeText.text = $"{data.minRoll} – {data.maxRoll}";

        //power — e.g. "+2" or "-1", hidden if zero
        if (powerText != null)
        {
            if (data.power != 0)
            {
                powerText.gameObject.SetActive(true);
                powerText.text = data.power > 0
                    ? $"+{data.power}"
                    : $"{data.power}";
            }
            else
            {
                powerText.gameObject.SetActive(false);
            }
        }

        //effect label
        if (effectText != null)
            effectText.text = data.effect.ToString();

        // Damage type icon
        if (icon != null)
            icon.sprite = data.damageType switch
            {
                DamageType.Slash  => slashSprite,
                DamageType.Pierce => pierceSprite,
                DamageType.Blunt  => bluntSprite,
                _                 => null
            };

        //background color by behaviour
        if (background != null)
            background.color = data.effect switch
            {
                DiceBehaviour.Attack => attackColor,
                DiceBehaviour.Defend => defendColor,
                DiceBehaviour.Evade  => evadeColor,
                DiceBehaviour.Buff   => buffColor,
                _                    => attackColor
            };
    }
}
