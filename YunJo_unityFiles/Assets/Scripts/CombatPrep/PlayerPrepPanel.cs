using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerPrepPanel : MonoBehaviour
{
    [Header("Slot Buttons")]
    public List<TeamSlotUI> slotUIs = new();

    [Header("Info Block — isInteractable must be true")]
    public UnitInfoBlock infoBlock;

    [Header("Enter Battle")]
    public Button enterBattleButton;

    CharacterUnit selectedUnit;

    void Start()
    {
        enterBattleButton?.onClick.AddListener(
            () => CombatPrepManager.Instance?.EnterBattle());
    }

    public void Bind(List<CharacterUnit> units)
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < units.Count)
            {
                bool isLeader = units[i].unitData != null &&
                                units[i].unitData.isLeader;
                slotUIs[i].BindPlayerUnit(units[i], isLeader, OnPlayerSelected);
            }
            else
            {
                slotUIs[i].BindEmpty();
            }
        }
    }

    void OnPlayerSelected(CharacterUnit unit)
    {
        selectedUnit = unit;

        foreach (var s in slotUIs)
            s.SetSelected(s.BoundUnit == unit);

        infoBlock?.BindUnit(unit);
        CombatPrepManager.Instance?.SelectUnit(unit);
    }

    public void SetSelected(CharacterUnit unit)
    {
        OnPlayerSelected(unit);
    }

    public void RefreshSelected()
    {
        infoBlock?.RefreshDeck();
    }
}
