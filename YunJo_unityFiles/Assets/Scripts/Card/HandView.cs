using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class HandView : MonoBehaviour
{
    [SerializeField] private float spacing = 120f;
    [SerializeField] private float hoverSpacing = 180f;

    private List<CardView> cards = new();
    private int hoveredIndex = -1;

    public void Register(CardView card)
    {
        if (card == null) return;

        cards.Add(card);

        card.onHoverEnter += OnHoverEnter;
        card.onHoverExit += OnHoverExit;

        UpdateLayout();
    }

    public void Clear()
    {
        foreach (var c in cards)
            if (c != null)
                Destroy(c.gameObject);

        cards.Clear();
    }

    void OnHoverEnter(CardView cv)
    {
        hoveredIndex = cards.IndexOf(cv);
        UpdateLayout();
    }

    void OnHoverExit(CardView cv)
    {
        hoveredIndex = -1;
        UpdateLayout();
    }

    public void UpdateLayout()
    {
        float center = (cards.Count - 1) / 2f;

        for (int i = 0; i < cards.Count; i++)
        {
            var c = cards[i];
            if (c == null) continue;

            float influence = 0;

            if (hoveredIndex != -1)
            {
                float dist = Mathf.Abs(i - hoveredIndex);
                influence = 1f - dist / cards.Count;
            }

            float s = Mathf.Lerp(spacing, hoverSpacing, influence);
            float x = (i - center) * s;

            Vector3 target = new Vector3(x, 0, 0);

            c.transform.DOKill();
            c.transform.DOLocalMove(target, 0.15f);
        }
    }
}
