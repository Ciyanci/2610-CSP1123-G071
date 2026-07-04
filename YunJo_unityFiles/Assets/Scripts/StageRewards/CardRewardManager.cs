using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class CardRewardManager : MonoBehaviour
{
    public static CardRewardManager Instance;

    [Header("Reward Pool — set per stage in Inspector")]
    public List<CardData> rewardPool = new();

    [Header("UI")]
    public GameObject rewardPanel;
    public Transform cardContainer;
    public RewardCardEntryUI cardEntryPrefab;
    public TextMeshProUGUI headerText;
    public Button skipButton;

    [Header("Save Key — unique per stage, prevents re-earning")]
    public string rewardSaveKey = "Tutorial1_CardReward";

    [Header("Next Scene")]
    public string nextSceneName = "MainMenu";

    bool rewardClaimed = false;

    void Awake()
    {
        Instance = this;
        rewardPanel?.SetActive(false);
    }

    void Start()
    {
        skipButton?.onClick.AddListener(Skip);
    }

    //called by TutorialManager or BattleResultsUI after victory
    public void ShowRewards()
    {
        //already claimed - skip straight to next scene
        if (PlayerPrefs.GetInt(rewardSaveKey, 0) == 1)
        {
            Debug.Log($"[REWARD] {rewardSaveKey} already claimed — skipping");
            Proceed();
            return;
        }

        rewardPanel?.SetActive(true);

        if (headerText != null)
            headerText.text = "Select a card to add to your collection";

        //spawn one entry per card in pool
        foreach (var card in rewardPool)
        {
            var entry = Instantiate(cardEntryPrefab, cardContainer);
            entry.Setup(card, OnCardSelected);
        }
    }

    void OnCardSelected(CardData card)
    {
        if (rewardClaimed) return;
        rewardClaimed = true;

        //save to PlayerPrefs so this reward can't be earned again
        PlayerPrefs.SetInt(rewardSaveKey, 1);

        //add to persistent card inventory
        CardInventory.Add(card);

        Debug.Log($"[REWARD] Player chose: {card.Name}");

        Proceed();
    }

    void Skip()
    {
        PlayerPrefs.SetInt(rewardSaveKey, 1);
        Proceed();
    }

    void Proceed()
    {
        rewardPanel?.SetActive(false);
        SceneManager.LoadScene(nextSceneName);
    }
}
