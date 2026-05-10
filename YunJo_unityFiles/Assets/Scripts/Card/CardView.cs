using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler
{
    [Header("Compact UI")]
    public TMP_Text title;
    public TMP_Text cost;
    public Image artwork;

    [Header("Compact Dice Icons")]
    public Transform diceIconsRow;
    public DiceIconUI diceIconPrefab;

    [Header("Expanded UI")]
    public GameObject expandedPanel;

    public RectTransform expandRoot;

    public TMP_Text desc;
    public TMP_Text effects;

    [Header("Expanded Dice List")]
    public Transform expandedDiceList;

    public ExpandedDiceRowUI expandedDicePrefab;

    [Header("Animation")]
    public float hoverHeight = 60f;
    public float expandOffset = -250f;
    public float animDuration = 0.18f;

    RectTransform rect;

    Vector2 originalPos;

    Card card;
    CharacterUnit owner;

    public CardFrameVisual frameVisual;

    bool hovered;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        if (expandedPanel != null)
            expandedPanel.SetActive(false);
    }

    void Start()
    {
        originalPos = rect.anchoredPosition;
    }

    public void Setup(Card c, CharacterUnit unit)
    {
        card = c;
        owner = unit;

        if (title != null)
            title.text = c.Data.Name;

        if (cost != null)
            cost.text = c.Cost.ToString();

        if (artwork != null)
        {
            artwork.sprite = c.Artwork;
            artwork.preserveAspect = true;
        }

        if (desc != null)
            desc.text = c.Data.Description;

        if (effects != null)
            effects.text = c.Data.Effects;

        BuildCompactDiceIcons();
        BuildExpandedDiceRows();

        if (frameVisual != null)
        {
            frameVisual.SetRarity(c.Data.rarity);
        }
    }

    void BuildCompactDiceIcons()
    {
        if (diceIconsRow == null ||
            diceIconPrefab == null)
            return;

        foreach (Transform child in diceIconsRow)
            Destroy(child.gameObject);

        foreach (var d in card.Data.dice)
        {
            DiceIconUI icon =
                Instantiate(
                    diceIconPrefab,
                    diceIconsRow
                );

            icon.Setup(d.damageType);
        }
    }

    void BuildExpandedDiceRows()
    {
        if (expandedDiceList == null ||
            expandedDicePrefab == null)
            return;

        foreach (Transform child in expandedDiceList)
            Destroy(child.gameObject);

        foreach (var d in card.Data.dice)
        {
            ExpandedDiceRowUI row =
                Instantiate(
                    expandedDicePrefab,
                    expandedDiceList
                );

            row.Setup(d);
        }
    }

    public void OnPointerEnter(
        PointerEventData eventData
    )
    {
        if (hovered)
            return;

        hovered = true;

        rect.DOKill();

        rect.DOAnchorPosY(
            originalPos.y + hoverHeight,
            animDuration
        ).SetEase(Ease.OutCubic);

        if (expandRoot != null)
        {
            expandRoot.DOKill();

            expandRoot.DOAnchorPosX(
                expandOffset,
                animDuration
            ).SetEase(Ease.OutCubic);
        }

        if (expandedPanel != null)
            expandedPanel.SetActive(true);

        HandUI.Instance?.OnCardHovered(this);
    }

    public void OnPointerExit(
        PointerEventData eventData
    )
    {
        Collapse();

        HandUI.Instance?.ResetHover();
    }

    public void OnPointerDown(
        PointerEventData eventData
    )
    {
        CombatFlowController.Instance
            .StartTargeting(card, owner);
    }

    public void Collapse()
    {
        hovered = false;

        rect.DOKill();

        rect.DOAnchorPosY(
            originalPos.y,
            animDuration
        ).SetEase(Ease.OutCubic);

        if (expandRoot != null)
        {
            expandRoot.DOKill();

            expandRoot.DOAnchorPosX(
                0,
                animDuration
            ).SetEase(Ease.OutCubic);
        }

        if (expandedPanel != null)
            expandedPanel.SetActive(false);
    }

    public void Shift(float amount)
    {
        rect.DOKill();

        rect.DOAnchorPosX(
            originalPos.x + amount,
            animDuration
        ).SetEase(Ease.OutCubic);
    }

    public void ResetShift()
    {
        if (hovered)
            return;

        rect.DOKill();

        rect.DOAnchorPosX(
            originalPos.x,
            animDuration
        ).SetEase(Ease.OutCubic);
    }

    public void ResetCard()
    {
        Collapse();
    }
}