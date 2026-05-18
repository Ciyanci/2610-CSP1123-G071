using UnityEngine;
using TMPro;

public class BattleResultsUI : MonoBehaviour
{
    public static BattleResultsUI Instance;

    public GameObject panel;
    public TextMeshProUGUI resultText;

    void Awake()
    {
        Instance = this;
        panel?.SetActive(false);
    }

    public void ShowVictory()   => Show("VICTORY");
    public void ShowDefeat()    => Show("DEFEAT");

    void Show(string msg)
    {
        panel?.SetActive(true);
        if (resultText != null) resultText.text = msg;
    }
}
