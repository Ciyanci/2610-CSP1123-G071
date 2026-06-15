using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TeamSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image           portrait;
    public TextMeshProUGUI nameText;
    public GameObject      leaderBadge;
    public GameObject      emptyState;
    public Image           selectedHighlight;
    public Button          button;
    public Button          swapButton;   // assistant only — opens unit picker

    [Header("Colors")]
    public Color selectedColor   = new Color(0.9f, 0.75f, 0.2f, 0.35f);
    public Color deselectedColor = new Color(0f, 0f, 0f, 0f);

    //bound data
    public TeamRosterSlot BoundSlot  { get; private set; }
    public UnitData       BoundEnemy { get; private set; }

    //bind for enemy to read
    public void BindEnemy(UnitData unit, Action<UnitData> onSelect)
    {
        BoundSlot  = null;
        BoundEnemy = unit;

        leaderBadge?.SetActive(false);
        swapButton?.gameObject.SetActive(false);
        emptyState?.SetActive(unit == null);

        if (unit != null)
        {
            if (portrait  != null && unit.portrait != null)
                portrait.sprite = unit.portrait;
            if (nameText  != null)
                nameText.text   = unit.unitName;
            portrait?.gameObject.SetActive(true);
        }

        button?.onClick.RemoveAllListeners();
        if (unit != null)
            button?.onClick.AddListener(() => onSelect?.Invoke(unit));
    }

    //bind player slots
    public void BindPlayerSlot(
        TeamRosterSlot slot,
        bool isLeader,
        int assistantIndex,
        Action<TeamRosterSlot> onSelect)
    {
        BoundSlot  = slot;
        BoundEnemy = null;

        leaderBadge?.SetActive(isLeader);
        emptyState?.SetActive(slot == null || slot.IsEmpty);

        //swap button only for empty/filled assistant slots
        swapButton?.gameObject.SetActive(!isLeader);
        swapButton?.onClick.RemoveAllListeners();
        if (!isLeader && assistantIndex >= 0)
            swapButton?.onClick.AddListener(() =>
                CombatPrepManager.Instance?.OpenUnitPickerWindow(assistantIndex));

        if (slot != null && !slot.IsEmpty)
        {
            if (portrait != null && slot.unit.portrait != null)
                portrait.sprite = slot.unit.portrait;
            if (nameText != null)
                nameText.text   = slot.unit.unitName;
            portrait?.gameObject.SetActive(true);
        }
        else
        {
            portrait?.gameObject.SetActive(false);
            if (nameText != null)
                nameText.text = isLeader ? "No Leader" : "Empty";
        }

        button?.onClick.RemoveAllListeners();
        button?.onClick.AddListener(() => onSelect?.Invoke(slot));
    }

    //bind for empty placeholders
    public void BindEmpty()
    {
        BoundSlot  = null;
        BoundEnemy = null;
        emptyState?.SetActive(true);
        portrait?.gameObject.SetActive(false);
        leaderBadge?.SetActive(false);
        swapButton?.gameObject.SetActive(false);
        button?.onClick.RemoveAllListeners();
    }

    public void SetSelected(bool selected)
    {
        if (selectedHighlight != null)
            selectedHighlight.color = selected ? selectedColor : deselectedColor;
    }
}
