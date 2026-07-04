using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InfoBarCardView : MonoBehaviour
{
    [Header("Base Info")]
    public TextMeshProUGUI cardName;
    public Image artwork;
    public TextMeshProUGUI costText;

    [Header("Dice Row")]
    public Transform diceRowContainer;     
    public InfoBarDiceRow diceRowPrefab;   

    List<InfoBarDiceRow> spawnedRows = new();

    public void Setup(Card card)
    {
        if (card == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (cardName != null) cardName.text = card.Name;
        if (artwork  != null) artwork.sprite = card.Artwork;
        if (costText != null) costText.text = card.Cost.ToString();

        //clear old rows
        foreach (var r in spawnedRows)
            if (r != null) Destroy(r.gameObject);
        spawnedRows.Clear();

        //spawn one row per die
        foreach (var die in card.GetDice())
        {
            var row = Instantiate(diceRowPrefab, diceRowContainer);
            row.Setup(die);
            spawnedRows.Add(row);
        }
    }

    public void Clear()
    {
        foreach (var r in spawnedRows)
            if (r != null) Destroy(r.gameObject);
        spawnedRows.Clear();

        gameObject.SetActive(false);
    }
}
