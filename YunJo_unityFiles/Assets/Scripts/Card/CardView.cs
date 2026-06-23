using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

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


    [Header("Dice Slot Row")]
    public List<DiceSlotUI> diceSlots = new(); 

    [Header("Expanded Dice Rows")]
    public List<ExpandedDiceRowUI> expandedDiceRows = new();


    //cached rarity visuals
    CardFrameVisual frameVisual;

    RectTransform rect;
    Vector2 basePos;

    Card card;
    public Card GetCard() => card;
    CharacterUnit owner;
    int siblingIndex;

    void Awake()
    {
        rect         = GetComponent<RectTransform>();
        frameVisual  = GetComponentInChildren<CardFrameVisual>(true);

        //disable raycasts on children so they don't eat pointer events from other cards (this caused me a headache)
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

        //makes so that only the root image receives raycasts (hopefully this one works)
        foreach (var g in GetComponentsInChildren<Graphic>())
            g.raycastTarget = false;

        var rootImage = GetComponent<Image>();
        if (rootImage != null)
            rootImage.raycastTarget = true;
    }
    public void BringToFront()
    {
        siblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
    }

    public void RestoreSiblingOrder()
    {
        transform.SetSiblingIndex(siblingIndex);
    }

    public void Setup(Card c, CharacterUnit unit)
    {
        card  = c;
        owner = unit;

        title.text     = c.Data.Name;
        cost.text      = c.Cost.ToString();
        artwork.sprite = c.Artwork;

        frameVisual?.SetRarity(c.Data.rarity);

        var dice = c.GetDice();

        //dice slots on the base card face
        if (diceSlots == null || diceSlots.Count == 0)
            Debug.LogWarning($"[CARDVIEW] {c.Data.Name}: diceSlots list is empty — " +
                            "wire DiceSlot child objects in the prefab Inspector");

        for (int i = 0; i < diceSlots.Count; i++)
        {
            if (diceSlots[i] == null) continue;
            if (i < dice.Count) diceSlots[i].Setup(dice[i]);
            else                diceSlots[i].Hide();
        }

        //expanded panel rows
        if (expandedDiceRows == null || expandedDiceRows.Count == 0)
            Debug.LogWarning($"[CARDVIEW] {c.Data.Name}: expandedDiceRows list is empty — " +
                            "wire DiceRow child objects in the prefab Inspector");

        for (int i = 0; i < expandedDiceRows.Count; i++)
        {
            if (expandedDiceRows[i] == null) continue;
            if (i < dice.Count)
            {
                expandedDiceRows[i].gameObject.SetActive(true);
                expandedDiceRows[i].Setup(dice[i]);
            }
            else
            {
                expandedDiceRows[i].gameObject.SetActive(false);
            }
        }

        if (expandedPanel != null)
            expandedPanel.SetActive(false);
    }


    //position methods
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

    public void LiftTo(float y)
    {
        rect.DOKill();
        rect.DOAnchorPos(new Vector2(basePos.x, basePos.y + y), 0.12f)
            .SetEase(Ease.OutCubic);
    }

    public void SetHover(bool value)
    {
        if (expandedPanel == null) return;
        expandedPanel.SetActive(value);
    }

    //pointer events
    public void OnPointerEnter(PointerEventData eventData)
    {
        CombatAudioManager.Instance?.PlayCardHover();
        HandUI.Instance?.OnCardHovered(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HandUI.Instance?.ResetHover();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CombatFlowController.Instance == null) return;
        CombatFlowController.Instance.StartTargeting(card, owner);
    }
}
