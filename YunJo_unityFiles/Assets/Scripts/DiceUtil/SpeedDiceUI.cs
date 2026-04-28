using UnityEngine;
using TMPro;

public class SpeedDiceUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CanvasGroup canvasGroup;

    Transform follow;
    Camera cam;
    RectTransform rect;

    void Awake()
    {
        cam = Camera.main;
        rect = transform as RectTransform;
    }

    public void Init(Transform target)
    {
        follow = target;
        Show();
    }

    void LateUpdate()
    {
        if (!follow) return;

        Vector3 screen = cam.WorldToScreenPoint(follow.position + Vector3.up * 1.2f);

        if (screen.z <= 0) return;

        rect.position = screen;
    }

    public void SetValue(int val)
    {
        text.text = val.ToString();
    }

    public void Show()
    {
        canvasGroup.alpha = 1;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
    }
}