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

    [Header("Colors")]
    public Color selectedColor   = new Color(0.9f, 0.75f, 0.2f, 0.35f);
    public Color deselectedColor = new Color(0f, 0f, 0f, 0f);

    public CharacterUnit BoundUnit  { get; private set; }
    public UnitData      BoundEnemy { get; private set; }

    public void BindEnemy(UnitData unit, Action<UnitData> onSelect)
    {
        BoundUnit  = null;
        BoundEnemy = unit;

        leaderBadge?.SetActive(false);
        emptyState?.SetActive(unit == null);

        if (unit != null)
        {
            if (portrait != null && unit.portrait != null)
                portrait.sprite = unit.portrait;
            if (nameText != null)
                nameText.text = unit.unitName;
            portrait?.gameObject.SetActive(true);
        }
        else
        {
            portrait?.gameObject.SetActive(false);
        }

        button?.onClick.RemoveAllListeners();
        if (unit != null)
            button?.onClick.AddListener(() => onSelect?.Invoke(unit));
    }

    public void BindPlayerUnit(CharacterUnit unit, bool isLeader,
                               Action<CharacterUnit> onSelect)
    {
        BoundUnit  = unit;
        BoundEnemy = null;

        leaderBadge?.SetActive(isLeader);
        emptyState?.SetActive(unit == null);

        if (unit != null)
        {
            if (portrait != null && unit.unitData?.portrait != null)
                portrait.sprite = unit.unitData.portrait;
            if (nameText != null)
                nameText.text = unit.unitName;
            portrait?.gameObject.SetActive(true);
        }
        else
        {
            portrait?.gameObject.SetActive(false);
            if (nameText != null)
                nameText.text = isLeader ? "No Leader" : "Empty";
        }

        button?.onClick.RemoveAllListeners();
        if (unit != null)
            button?.onClick.AddListener(() => onSelect?.Invoke(unit));
    }

    public void BindEmpty()
    {
        BoundUnit  = null;
        BoundEnemy = null;
        emptyState?.SetActive(true);
        portrait?.gameObject.SetActive(false);
        leaderBadge?.SetActive(false);
        button?.onClick.RemoveAllListeners();
    }

    public void SetSelected(bool selected)
    {
        if (selectedHighlight != null)
            selectedHighlight.color = selected ? selectedColor : deselectedColor;
    }
}
