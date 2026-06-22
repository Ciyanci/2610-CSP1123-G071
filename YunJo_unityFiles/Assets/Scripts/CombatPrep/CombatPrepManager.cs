using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CombatPrepManager : MonoBehaviour
{
    public static CombatPrepManager Instance;

    [Header("Scene To Load")]
    public string combatSceneName = "Combat_Stage1";

    [Header("Player Units — assign in Inspector")]
    public List<CharacterUnit> playerUnits = new();

    [Header("All Available Cards")]
    public List<CardData> allCards = new();

    [Header("All Available Keypages")]
    public List<KeypageData> allKeypages = new();

    [Header("UI")]
    public PlayerPrepPanel    playerPanel;
    public EnemyPrepPanel     enemyPanel;
    public KeypageWindowUI    keypageWindow;
    public CardEditorWindowUI cardEditorWindow;

    [Header("Stage Title")]
    public TMPro.TextMeshProUGUI stageTitleText;

    [Header("Enemy Preview Units — assign UnitData directly")]
    public List<UnitData> enemyPreviewData = new();

    CharacterUnit selectedUnit;

    void Awake() => Instance = this;

    void Start()
    {
        keypageWindow?.Close();
        cardEditorWindow?.Close();

        enemyPanel?.Bind(enemyPreviewData);
        playerPanel?.Bind(playerUnits);

        //auto-select first player unit
        if (playerUnits.Count > 0)
            SelectUnit(playerUnits[0]);
    }

    //selection
    public void SelectUnit(CharacterUnit unit)
    {
        selectedUnit = unit;
        playerPanel?.SetSelected(unit);
    }

    public CharacterUnit GetSelectedUnit() => selectedUnit;

    //windows
    public void OpenKeypageWindow(CharacterUnit unit)
    {
        if (unit == null) return;
        if (unit.unitData != null && unit.unitData.isLeader)
        {
            Debug.Log("[PREP] Leader keypage is locked");
            return;
        }
        keypageWindow?.Open(unit, allKeypages);
    }

    public void OpenCardEditorWindow(CharacterUnit unit)
    {
        if (unit == null) return;
        cardEditorWindow?.Open(unit, allCards);
    }

    public void CloseAllWindows()
    {
        keypageWindow?.Close();
        cardEditorWindow?.Close();
    }

    //deck editing
    public void AddCard(CharacterUnit unit, CardData card)
    {
        if (unit?.deck == null || card == null) return;
        if (!unit.deck.startingDeck.Contains(card))
            unit.deck.startingDeck.Add(card);
        cardEditorWindow?.Refresh();
        playerPanel?.RefreshSelected();
    }

    public void RemoveCard(CharacterUnit unit, CardData card)
    {
        if (unit?.deck == null || card == null) return;
        unit.deck.startingDeck.Remove(card);
        cardEditorWindow?.Refresh();
        playerPanel?.RefreshSelected();
    }

    //keypage
    public void EquipKeypage(CharacterUnit unit, KeypageData keypage)
    {
        if (unit == null) return;
        if (unit.unitData != null && unit.unitData.isLeader) return;
        unit.equippedKeypage = keypage;
        keypageWindow?.Refresh();
        playerPanel?.RefreshSelected();
    }

    //enter battle
    public void EnterBattle()
    {
        SceneManager.LoadScene(combatSceneName);
    }
}
