using UnityEngine;
using System.Collections.Generic;

public class EnemyPrepPanel : MonoBehaviour
{
    [Header("Slot Buttons")]
    public List<TeamSlotUI> slotUIs = new();

    [Header("Info Block")]
    public UnitInfoBlock infoBlock;

    public void Bind(List<UnitData> units)
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i < units.Count)
                slotUIs[i].BindEnemy(units[i], OnEnemySelected);
            else
                slotUIs[i].BindEmpty();
        }

        if (units.Count > 0)
            OnEnemySelected(units[0]);
    }

    void OnEnemySelected(UnitData unit)
    {
        foreach (var s in slotUIs)
            s.SetSelected(s.BoundEnemy == unit);

        infoBlock?.BindEnemy(unit);
    }
}
