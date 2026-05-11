using UnityEngine;
using TMPro;

public class TargetPreviewUI : MonoBehaviour
{
    public static TargetPreviewUI Instance;

    public TextMeshProUGUI text;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(string msg, Color color)
    {
        gameObject.SetActive(true);
        text.text = msg;
        text.color = color;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}