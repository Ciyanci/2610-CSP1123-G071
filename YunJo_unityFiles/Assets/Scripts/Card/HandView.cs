using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using DG.Tweening;

public class HandView : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private float baseSpacing = 0.08f;
    [SerializeField] private float hoverSpacing = 0.18f;

    private int hoveredIndex = -1;
    private readonly List<CardView> cards = new();
    public IEnumerator AddCard(CardView cardView)
    {
        cards.Add(cardView);
        yield return UpdateCardPositions(0.15f);
    }
    private IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0) yield break;

        float center = (cards.Count - 1) / 2f;

        Spline spline = splineContainer.Spline;

        for (int i = 0; i < cards.Count; i++)
        {
            // -----------------------------
            // 🧠 HOVER-BASED SPACING
            // -----------------------------

            float spacing = baseSpacing;

            if (hoveredIndex != -1)
            {
                float dist = Mathf.Abs(i - hoveredIndex);
                float influence = 1f - (dist / (float)cards.Count);

                spacing = Mathf.Lerp(baseSpacing, hoverSpacing, influence);
            }

            float offset = (i - center) * spacing;

            float p = 0.5f + offset;

            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);

            Quaternion rotation = Quaternion.LookRotation(
                -up,
                Vector3.Cross(-up, forward).normalized
            );

            Vector3 finalPos =
                splinePosition +
                transform.position +
                0.01f * i * Vector3.back;

            cards[i].transform.DOMove(finalPos, duration);
            cards[i].transform.DORotate(rotation.eulerAngles, duration);
        }

        yield return new WaitForSeconds(duration);
    }
}
