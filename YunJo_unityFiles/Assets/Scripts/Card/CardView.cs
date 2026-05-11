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

    RectTransform rect;

    Vector2 basePos;

    Card card;
    public Card GetCard() => card;
    CharacterUnit owner;

    bool isHovered;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        if (expandedPanel != null)
        {
            expandedPanel.SetActive(false);

            CanvasGroup cg = expandedPanel.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = expandedPanel.AddComponent<CanvasGroup>();

            cg.blocksRaycasts = false;
            cg.interactable = false;
            cg.ignoreParentGroups = true;
        }

        foreach (var g in GetComponentsInChildren<Graphic>())
            g.raycastTarget = false;

        var rootImage = GetComponent<Image>();
        if (rootImage != null)
            rootImage.raycastTarget = true;
    }

    void RefreshExpandedUI()
    {
        if (card == null) return;
    }
    public void Setup(Card c, CharacterUnit unit)
    {
        card = c;
        owner = unit;

        title.text = c.Data.Name;
        cost.text = c.Cost.ToString();
        artwork.sprite = c.Artwork;

        if (expandedPanel != null)
        {
            expandedPanel.SetActive(false);
            expandedPanel.SetActive(true);
            expandedPanel.SetActive(false);
        }
    }

    // =========================
    // HAND OWNED POSITION
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

        isHovered = false;
    }

    // =========================
    // SHIFT (ONLY HORIZONTAL OFFSET)
    // =========================
    public void ApplyShift(float x)
    {
        rect.DOKill();

        rect.DOAnchorPosX(basePos.x + x, 0.12f)
            .SetEase(Ease.OutCubic);
    }

    // =========================
    // HOVER VISUAL ONLY
    // =========================
    public void SetHover(bool value)
    {
        isHovered = value;

        if (value)
        {
            if (expandedPanel != null)
            {
                expandedPanel.SetActive(true);
                RefreshExpandedUI(); // 🔥 FORCE SYNC HERE
            }
        }
        else
        {
            if (expandedPanel != null)
                expandedPanel.SetActive(false);
        }
    }
    // =========================
    // INPUT
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
        Debug.Log("[CARD CLICK] " + card.Data.Name);

        CombatFlowController.Instance.StartTargeting(card, owner);
    }
}