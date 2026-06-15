using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CombatPrepManager : MonoBehaviour
{
    public static CombatPrepManager Instance;

    [Header("Data")]
    public TeamRoster        roster;
    public StageData         stage;
    public List<CardData>    allCards     = new();
    public List<KeypageData> allKeypages  = new();
    public List<UnitData>    allUnits     = new();  //for assistant picker

    [Header("Panels")]
    public EnemyPrepPanel  enemyPanel;
    public PlayerPrepPanel playerPanel;

    [Header("Stage Title")]
    public TMPro.TextMeshProUGUI stageTitleText;

    [Header("Overlay Windows")]
    public KeypageWindowUI    keypageWindow;
    public CardEditorWindowUI cardEditorWindow;
    public UnitPickerWindowUI unitPickerWindow;

    //currently selected player slot
    TeamRosterSlot selectedPlayerSlot;

    void Awake() => Instance = this;

    void Start()
    {
        if (BattleContext.ActiveStage  != null) stage  = BattleContext.ActiveStage;
        if (BattleContext.ActiveRoster != null) roster = BattleContext.ActiveRoster;

        if (stage  == null) { Debug.LogError("[PREP] No stage");  return; }
        if (roster == null) { Debug.LogError("[PREP] No roster"); return; }

        //ensure leader deck initialized
        if (!roster.leaderSlot.IsEmpty &&
            roster.leaderSlot.configuredDeck.Count == 0)
            roster.leaderSlot.InitializeDeck();

        if (stageTitleText != null)
            stageTitleText.text = stage.stageName;

        //close all windows
        keypageWindow?.Close();
        cardEditorWindow?.Close();
        unitPickerWindow?.Close();

        enemyPanel?.Bind(stage.enemyUnits);
        playerPanel?.Bind(roster);

        //auto-select leader
        SelectPlayerSlot(roster.leaderSlot);
    }

    //selection
    public void SelectPlayerSlot(TeamRosterSlot slot)
    {
        selectedPlayerSlot = slot;
        playerPanel?.SetSelected(slot);
    }

    public TeamRosterSlot GetSelectedPlayerSlot() => selectedPlayerSlot;

    //windows
    public void OpenKeypageWindow(TeamRosterSlot slot)
    {
        if (slot == null || slot.IsEmpty) return;
        if (slot.unit.isLeader)
        {
            Debug.Log("[PREP] Leader keypage locked");
            return;
        }
        keypageWindow?.Open(slot, allKeypages);
    }

    public void OpenCardEditorWindow(TeamRosterSlot slot)
    {
        if (slot == null || slot.IsEmpty) return;
        cardEditorWindow?.Open(slot, allCards);
    }

    public void OpenUnitPickerWindow(int assistantIndex)
    {
        unitPickerWindow?.Open(assistantIndex, allUnits, roster);
    }

    public void CloseAllWindows()
    {
        keypageWindow?.Close();
        cardEditorWindow?.Close();
        unitPickerWindow?.Close();
    }

    //deck editing
    public void AddCard(TeamRosterSlot slot, CardData card)
    {
        if (slot == null || slot.IsEmpty || card == null) return;
        if (!slot.configuredDeck.Contains(card))
            slot.configuredDeck.Add(card);

        cardEditorWindow?.Refresh();
        playerPanel?.RefreshSelected();
    }

    public void RemoveCard(TeamRosterSlot slot, CardData card)
    {
        if (slot == null || slot.IsEmpty || card == null) return;
        slot.configuredDeck.Remove(card);

        cardEditorWindow?.Refresh();
        playerPanel?.RefreshSelected();
    }

    //keypage
    public void EquipKeypage(TeamRosterSlot slot, KeypageData keypage)
    {
        if (slot == null || slot.IsEmpty || slot.unit.isLeader) return;
        slot.equippedKeypage = keypage;

        keypageWindow?.Refresh();
        playerPanel?.RefreshSelected();
    }

    //unit swap
    public void AssignUnit(int assistantIndex, UnitData unit)
    {
        if (assistantIndex < 0 ||
            assistantIndex >= roster.assistantSlots.Length) return;

        var slot = roster.assistantSlots[assistantIndex];
        slot.unit            = unit;
        slot.equippedKeypage = null;
        slot.InitializeDeck();

        playerPanel?.Bind(roster);
        SelectPlayerSlot(slot);
        unitPickerWindow?.Close();
    }

    public void ClearAssistantSlot(int assistantIndex)
    {
        if (assistantIndex < 0 ||
            assistantIndex >= roster.assistantSlots.Length) return;

        var slot = roster.assistantSlots[assistantIndex];
        slot.unit            = null;
        slot.equippedKeypage = null;
        slot.configuredDeck.Clear();

        playerPanel?.Bind(roster);
        SelectPlayerSlot(roster.leaderSlot);
        unitPickerWindow?.Close();
    }

    //enter battle
    public void EnterBattle()
    {
        if (!roster.IsValid())
        {
            Debug.LogWarning("[PREP] Need at least a leader");
            return;
        }

        BattleContext.Set(stage, roster);
        SceneManager.LoadScene(stage.combatSceneName);
    }
}
