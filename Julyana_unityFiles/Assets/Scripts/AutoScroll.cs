using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class AutoScrollToSelected : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;

    private RectTransform selectedRect;

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null) return;

        RectTransform rect = selected.GetComponent<RectTransform>();

        if (rect == null || rect == selectedRect) return;

        selectedRect = rect;

        ScrollToSelected(rect);
    }

    void ScrollToSelected(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 position = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) 
        - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

        contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x,Mathf.Clamp(position.y,0,contentPanel.sizeDelta.y));
    }
}
