using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FrameButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Button Info")]
    public string title = "Button Title";
    [TextArea] public string description = "Details about this button...";

    [Header("Visual Novel")]
    public StoryScene storyScene; // ← new

    private MainFrameController controller;

    void Awake()
    {
        controller = GetComponentInParent<MainFrameController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        controller.OnFrameButtonClicked(this);
    }
}