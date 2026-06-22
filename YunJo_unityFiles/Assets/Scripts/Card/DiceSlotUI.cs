using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceSlotUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text   rangeText;
    public DiceIconUI iconUI;
    public Image      background;

    [Header("Behaviour Colors")]
    public Color attackColor = new Color(0.75f, 0.15f, 0.15f, 1f);
    public Color defendColor = new Color(0.15f, 0.35f, 0.75f, 1f);
    public Color evadeColor  = new Color(0.15f, 0.65f, 0.35f, 1f);
    public Color buffColor   = new Color(0.65f, 0.55f, 0.15f, 1f);

    public void Setup(DiceData data)
    {
        gameObject.SetActive(true);

        if (rangeText != null)
            rangeText.text = $"{data.minRoll}–{data.maxRoll}";

        iconUI?.Setup(data.damageType);

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

    public void Hide() => gameObject.SetActive(false);
}
