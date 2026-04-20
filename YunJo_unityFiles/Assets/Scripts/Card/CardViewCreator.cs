using UnityEngine;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;

    public CardView CreateCardView()
    {
        CardView cardView = Instantiate(cardViewPrefab);

        cardView.transform.SetParent(null);
        cardView.transform.localScale = Vector3.zero;
        cardView.transform.position = Vector3.zero;

        return cardView;
    }
}