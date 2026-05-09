using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverColor : MonoBehaviour
{
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;

    void Start()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button btn in allButtons)
        {
            TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
            if (text == null) continue;

            EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();

            Color normal = normalColor;
            Color hover = hoverColor;
            TMP_Text capturedText = text;

            AddTrigger(trigger, EventTriggerType.PointerEnter, () => capturedText.color = hover);
            AddTrigger(trigger, EventTriggerType.PointerExit, () => capturedText.color = normal);
        }
    }

    void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => action());
        trigger.triggers.Add(entry);
    }
}