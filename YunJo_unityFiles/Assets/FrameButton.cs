using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Attach this to each button inside MainFrame
public class FrameButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Button Info")]
    public string title = "Button Title";
    [TextArea] public string description = "Details about this button...";

    private MainFrameController controller;

    void Awake()
    {
        // Walk up to find the controller on the MainFrame
        controller = GetComponentInParent<MainFrameController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        controller.OnFrameButtonClicked(this);
    }
}