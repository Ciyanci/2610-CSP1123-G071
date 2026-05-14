// CardView.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardView : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler
{
    [Header("UI")]
    public TMP_Text title;
    public TMP_Text cost;
    public Image artwork;

    [Header("Expanded")]
    public GameObject expandedPanel;

    // Cached rarity visual — found at Awake
    CardFrameVisual frameVisual;

    RectTransform rect;
    Vector2 basePos;

    Card card;
    public Card GetCard() => card;
    CharacterUnit owner;

    void Awake()
    {
        rect         = GetComponent<RectTransform>();
        frameVisual  = GetComponentInChildren<CardFrameVisual>(true);

        // Expanded panel setup — disable raycasts on children so they
        // don't eat pointer events from other cards
        if (expandedPanel != null)
        {
            expandedPanel.SetActive(false);

            CanvasGroup cg = expandedPanel.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = expandedPanel.AddComponent<CanvasGroup>();

            cg.blocksRaycasts = false;
            cg.interactable   = false;
            cg.ignoreParentGroups = true;
        }

        // Only the root Image receives raycasts — children are purely visual
        foreach (var g in GetComponentsInChildren<Graphic>())
            g.raycastTarget = false;

        var rootImage = GetComponent<Image>();
        if (rootImage != null)
            rootImage.raycastTarget = true;
    }

    public void Setup(Card c, CharacterUnit unit)
    {
        card  = c;
        owner = unit;

        title.text   = c.Data.Name;
        cost.text    = c.Cost.ToString();
        artwork.sprite = c.Artwork;

        // Apply rarity shader now that card data is known
        frameVisual?.SetRarity(c.Data.rarity);

        if (expandedPanel != null)
            expandedPanel.SetActive(false);
    }

    // =========================
    // POSITION
    // =========================
    public void SetBasePosition(Vector2 pos)
    {
        basePos = pos;
        rect.anchoredPosition = pos;
    }

    public void ResetToBase()
    {
        rect.DOKill();
        rect.DOAnchorPos(basePos, 0.12f).SetEase(Ease.OutCubic);

        if (expandedPanel != null)
            expandedPanel.SetActive(false);
    }

    public void ApplyShift(float x)
    {
        rect.DOKill();
        rect.DOAnchorPosX(basePos.x + x, 0.12f).SetEase(Ease.OutCubic);
    }

    // Add inside CardView alongside ApplyShift:
    public void LiftTo(float y)
    {
        rect.DOKill();
        rect.DOAnchorPos(new Vector2(basePos.x, basePos.y + y), 0.12f)
            .SetEase(Ease.OutCubic);
    }

    public void SetHover(bool value)
    {
        if (expandedPanel != null)
            expandedPanel.SetActive(value);
    }

    // =========================
    // POINTER EVENTS
    // =========================
    public void OnPointerEnter(PointerEventData eventData)
    {
        HandUI.Instance?.OnCardHovered(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HandUI.Instance?.ResetHover();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CombatFlowController.Instance.StartTargeting(card, owner);
    }
}
