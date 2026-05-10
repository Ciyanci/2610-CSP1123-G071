using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpandedDiceRowUI : MonoBehaviour
{
    public Image icon;

    public TMP_Text rangeText;
    public TMP_Text effectText;

    public Sprite slashSprite;
    public Sprite pierceSprite;
    public Sprite bluntSprite;

    public void Setup(DiceData data)
    {
        rangeText.text =
            $"{data.minRoll}-{data.maxRoll}";

        effectText.text =
            data.effect.ToString(); // 🔥 FIX

        switch (data.damageType)
        {
            case DamageType.Slash:
                icon.sprite = slashSprite;
                break;

            case DamageType.Pierce:
                icon.sprite = pierceSprite;
                break;

            case DamageType.Blunt:
                icon.sprite = bluntSprite;
                break;
        }
    }
}