// CombatCardDisplayUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatCardDisplayUI : MonoBehaviour
{
    public static CombatCardDisplayUI Instance;

    [Header("Left (Player)")]
    public GameObject leftRoot;
    public Image leftArtwork;
    public TextMeshProUGUI leftCardName;

    [Header("Right (Enemy)")]
    public GameObject rightRoot;
    public Image rightArtwork;
    public TextMeshProUGUI rightCardName;

    [Header("Follow Settings")]
    public Vector3 behindOffset = new Vector3(0f, 0.5f, 0f);

    CharacterUnit _unitA;
    CharacterUnit _unitB;

    Camera cam;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;
        Hide();
    }

    public void ShowForClash(CharacterUnit unitA, Card cardA, CharacterUnit unitB, Card cardB)
    {
        _unitA = unitA;
        _unitB = unitB;

        leftRoot?.SetActive(true);
        rightRoot?.SetActive(true);

        BindSide(leftArtwork,  leftCardName,  cardA);
        BindSide(rightArtwork, rightCardName, cardB);
    }

    public void ShowForUnopposed(CharacterUnit attacker, Card card)
    {
        _unitA = attacker;
        _unitB = null;

        leftRoot?.SetActive(true);
        rightRoot?.SetActive(false);

        BindSide(leftArtwork, leftCardName, card);
    }

    public void Hide()
    {
        _unitA = null;
        _unitB = null;

        leftRoot?.SetActive(false);
        rightRoot?.SetActive(false);
    }

    void BindSide(Image artwork, TextMeshProUGUI nameText, Card card)
    {
        if (card == null) return;
        if (artwork  != null) artwork.sprite = card.Artwork;
        if (nameText != null) nameText.text  = card.Name;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        if (_unitA != null && leftRoot != null && leftRoot.activeSelf)
            leftRoot.transform.position = _unitA.visual.position + behindOffset;

        if (_unitB != null && rightRoot != null && rightRoot.activeSelf)
            rightRoot.transform.position = _unitB.visual.position + behindOffset;
    }
}