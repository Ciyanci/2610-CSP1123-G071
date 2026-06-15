using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitPickerEntryUI : MonoBehaviour
{
    public Image           portrait;
    public TextMeshProUGUI nameText;
    public Button          selectButton;

    public void Setup(UnitData unit, int assistantIndex)
    {
        if (portrait != null && unit.portrait != null)
            portrait.sprite = unit.portrait;
        if (nameText != null)
            nameText.text = unit.unitName;

        selectButton?.onClick.RemoveAllListeners();
        selectButton?.onClick.AddListener(() =>
            CombatPrepManager.Instance?.AssignUnit(assistantIndex, unit));
    }

    public void SetupClear(int assistantIndex)
    {
        if (portrait != null) portrait.gameObject.SetActive(false);
        if (nameText != null) nameText.text = "— Clear Slot —";

        selectButton?.onClick.RemoveAllListeners();
        selectButton?.onClick.AddListener(() =>
            CombatPrepManager.Instance?.ClearAssistantSlot(assistantIndex));
    }
}
