using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceSlotUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text rangeText;
    public DiceIconUI iconUI;

    public void Setup(DiceData data)
    {
        gameObject.SetActive(true);

        if (rangeText != null)
            rangeText.text = $"{data.minRoll}-{data.maxRoll}";

        if (iconUI != null)
            iconUI.Setup(data.damageType);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}