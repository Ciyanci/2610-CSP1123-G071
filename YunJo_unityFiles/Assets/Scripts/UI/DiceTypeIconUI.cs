using UnityEngine;
using UnityEngine.UI;

public class DiceTypeIconUI : MonoBehaviour
{
    [Header("Refs")]
    public Image typeIcon;
    public Image background;
    public Image highlight;     //bright border — active die indicator

    [Header("Type Sprites")]
    public Sprite slashSprite;
    public Sprite pierceSprite;
    public Sprite bluntSprite;

    [Header("Behaviour Colors")]
    public Color attackColor = new Color(0.75f, 0.15f, 0.15f, 1f);
    public Color defendColor = new Color(0.15f, 0.35f, 0.75f, 1f);
    public Color evadeColor  = new Color(0.15f, 0.65f, 0.35f, 1f);
    public Color buffColor   = new Color(0.65f, 0.55f, 0.15f, 1f);

    public void Setup(DiceData data)
    {
        gameObject.SetActive(true);

        if (typeIcon != null)
            typeIcon.sprite = data.damageType switch
            {
                DamageType.Slash  => slashSprite,
                DamageType.Pierce => pierceSprite,
                DamageType.Blunt  => bluntSprite,
                _                 => null
            };

        if (background != null)
            background.color = data.effect switch
            {
                DiceBehaviour.Attack => attackColor,
                DiceBehaviour.Defend => defendColor,
                DiceBehaviour.Evade  => evadeColor,
                DiceBehaviour.Buff   => buffColor,
                _                   => attackColor
            };

        SetHighlight(false);
        SetSpent(false);
    }

    //bright border when this die is currently resolving
    public void SetHighlight(bool active)
    {
        if (highlight != null)
            highlight.gameObject.SetActive(active);
    }

    //grey out when die is resolved/broken
    public void SetSpent(bool spent = true)
    {
        if (background != null)
            background.color = spent
                ? new Color(0.2f, 0.2f, 0.2f, 0.5f)
                : background.color;

        if (typeIcon != null)
            typeIcon.color = spent
                ? new Color(1f, 1f, 1f, 0.25f)
                : Color.white;

        SetHighlight(false);
    }
}