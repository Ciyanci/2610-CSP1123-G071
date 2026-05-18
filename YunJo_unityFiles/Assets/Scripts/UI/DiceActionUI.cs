using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceActionIconUI : MonoBehaviour
{
    [Header("Refs")]
    public Image typeIcon;
    public Image background;
    public Image activeHighlight;   //bright border shown on the currently-resolving die
    public TextMeshProUGUI powerText; //shows +power value e.g. "+2"

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

        if (background != null)
            background.color = ColorForBehaviour(data.effect);

        if (typeIcon != null)
            typeIcon.sprite = data.damageType switch
            {
                DamageType.Slash  => slashSprite,
                DamageType.Pierce => pierceSprite,
                DamageType.Blunt  => bluntSprite,
                _                 => null
            };

        if (powerText != null)
            powerText.text = data.power >= 0
                ? $"+{data.power}"
                : $"{data.power}";

        SetActive(false);
    }

    //highlight this icon when its die is currently resolving
    public void SetActive(bool active)
    {
        if (activeHighlight != null)
            activeHighlight.gameObject.SetActive(active);
    }

    //grey out when die is spent/broken
    public void SetSpent()
    {
        if (background != null)
            background.color = new Color(0.25f, 0.25f, 0.25f, 0.6f);

        if (typeIcon != null)
            typeIcon.color = new Color(1f, 1f, 1f, 0.3f);

        SetActive(false);
    }

    public void Hide() => gameObject.SetActive(false);

    Color ColorForBehaviour(DiceBehaviour b) => b switch
    {
        DiceBehaviour.Attack => attackColor,
        DiceBehaviour.Defend => defendColor,
        DiceBehaviour.Evade  => evadeColor,
        DiceBehaviour.Buff   => buffColor,
        _                    => attackColor
    };
}