using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class StretchMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [Header("References")]
    public RectTransform buttonRect;
    public RectTransform textBGRect;
    public Image buttonBG;
    public CanvasGroup buttonBGGroup;
    public Image leftBorder;
    public TextMeshProUGUI label;

    [Header("Settings")]
    public float defaultWidth = 800f;
    public float hoverWidth = 1920f;
    public float textBGWidth = 1500f;
    public float textBGDefault = 150f;
    public float animDuration = 0.2f;

    [Header("Colors")]
    public Color defaultBG = new Color (0.50f, 0.50f, 0.50f, 1.00f);
    public Color hoverBG = new Color (0.00f, 1.00f, 0.91f, 1.00f);
    public Color defaultLabel = new Color (1.00f, 1.00f, 1.00f, 0.70f);
    public Color hoverLabel = new Color (1.00f, 1.00f, 1.00f, 0.70f);
    public Color borderColor = new Color (0.0f, 1.00f, 0.91f, 1.00f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonRect.sizeDelta = new Vector2(defaultWidth,buttonRect.sizeDelta.y);
        textBGRect.sizeDelta = new Vector2(textBGDefault, textBGRect.sizeDelta.y);
        buttonBG.color = defaultBG;
        buttonBGGroup.alpha = 1f;
        label.color = defaultLabel;
        leftBorder.color = new Color(borderColor.r, borderColor.g, borderColor.b,0f);
    }

    // Update is called once per frame
    public void OnPointerEnter(PointerEventData e)
    {
        DOTween.Kill(gameObject);
        buttonRect.DOSizeDelta(new Vector2(hoverWidth, buttonRect.sizeDelta.y), animDuration)
            .SetEase(Ease.OutCubic).SetTarget(gameObject);

        textBGRect.DOSizeDelta(new Vector2(textBGWidth, textBGRect.sizeDelta.y), animDuration)
            .SetEase(Ease.OutCubic).SetTarget(gameObject);

        buttonBG.DOColor(hoverBG, animDuration).SetTarget(gameObject);
        label.DOColor(hoverLabel, 0.15f).SetTarget(gameObject);
        leftBorder.DOFade(1f, 0.15f).SetTarget(gameObject);
    }

    public void OnPointerExit(PointerEventData e)
    {
        DOTween.Kill(gameObject);
        buttonRect.DOSizeDelta(new Vector2(defaultWidth, buttonRect.sizeDelta.y), animDuration)
            .SetEase(Ease.OutCubic).SetTarget(gameObject);
        
        textBGRect.DOSizeDelta(new Vector2(textBGDefault, textBGRect.sizeDelta.y), animDuration)
            .SetEase(Ease.OutCubic).SetTarget(gameObject);

        buttonBG.DOColor(defaultBG, animDuration).SetTarget(gameObject);
        label.DOColor(defaultLabel, 0.15f).SetTarget(gameObject);
        leftBorder.DOFade(0f, 0.15f).SetTarget(gameObject);
    }
}
