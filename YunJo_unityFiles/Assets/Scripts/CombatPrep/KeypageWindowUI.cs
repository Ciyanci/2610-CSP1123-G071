using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class KeypageWindowUI : MonoBehaviour
{
    public Transform      container;
    public KeypageEntryUI entryPrefab;
    public Button         closeButton;

    List<KeypageEntryUI> spawned = new();
    CharacterUnit        boundUnit;
    List<KeypageData>    keypages;

    void Awake() => closeButton?.onClick.AddListener(Close);

    public void Open(CharacterUnit unit, List<KeypageData> available)
    {
        boundUnit = unit;
        keypages  = available;
        gameObject.SetActive(true);
        Refresh();
    }

    public void Refresh()
    {
        foreach (var e in spawned)
            if (e != null) Destroy(e.gameObject);
        spawned.Clear();

        if (boundUnit == null || keypages == null) return;

        foreach (var kp in keypages)
        {
            var entry    = Instantiate(entryPrefab, container);
            bool equipped = boundUnit.equippedKeypage == kp;
            entry.Setup(kp, equipped, boundUnit);
            spawned.Add(entry);
        }
    }

    public void Close() => gameObject.SetActive(false);
}
