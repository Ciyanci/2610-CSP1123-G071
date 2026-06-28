using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Tutorial Steps")]
    public List<TutorialStep> steps = new();

    [Header("Prompt UI")]
    public GameObject        promptPanel;
    public TextMeshProUGUI   promptText;
    public Button            continueButton;

    [Header("Highlight Overlay")]
    // Optional — a transparent colored panel you move over UI elements
    public RectTransform     highlightRect;

    int currentStep = 0;
    bool waitingForContinue = false;

    void Awake() => Instance = this;

    void Start()
    {
        // Block input until tutorial grants it
        CombatFlowController.Instance.SetInputEnabled(false);
        promptPanel?.SetActive(false);
        continueButton?.onClick.AddListener(OnContinue);
        StartCoroutine(RunTutorial());
    }

    // =========================
    // TUTORIAL LOOP
    // =========================
    IEnumerator RunTutorial()
    {
        foreach (var step in steps)
        {
            currentStep++;

            // Show prompt
            if (!string.IsNullOrEmpty(step.promptText))
            {
                ShowPrompt(step.promptText);
                yield return new WaitUntil(() => !waitingForContinue);
            }

            // Grant input if step requires it
            if (step.enableInput)
                CombatFlowController.Instance.SetInputEnabled(true);

            // Wait for a game condition if specified
            if (step.waitCondition != TutorialWaitCondition.None)
                yield return WaitForCondition(step.waitCondition);

            // Block input again after condition met (unless step says keep it)
            if (step.enableInput && !step.keepInputAfter)
                CombatFlowController.Instance.SetInputEnabled(false);
        }

        // All steps done — hand fully to player
        CombatFlowController.Instance.SetInputEnabled(true);
        promptPanel?.SetActive(false);
    }

    // =========================
    // WAIT CONDITIONS
    // =========================
    IEnumerator WaitForCondition(TutorialWaitCondition condition)
    {
        switch (condition)
        {
            case TutorialWaitCondition.PlayerAssignedCard:
                yield return new WaitUntil(() =>
                    AnyPlayerSlotPlanned());
                break;

            case TutorialWaitCondition.PlayerConfirmedTurn:
                yield return new WaitUntil(() =>
                    !CombatFlowController.Instance.inputEnabled);
                break;

            case TutorialWaitCondition.TurnResolved:
                yield return new WaitUntil(() =>
                    CombatFlowController.Instance.inputEnabled);
                break;

            case TutorialWaitCondition.EnemyDead:
                yield return new WaitUntil(() =>
                    UnitRegistry.Instance.enemies.TrueForAll(
                        e => e == null || e.IsDead));
                break;
        }
    }

    bool AnyPlayerSlotPlanned()
    {
        foreach (var unit in UnitRegistry.Instance.players)
            foreach (var slot in unit.speedSlots)
                if (slot.state == SlotState.Planned ||
                    slot.state == SlotState.Committed)
                    return true;
        return false;
    }

    // =========================
    // PROMPT
    // =========================
    void ShowPrompt(string text)
    {
        waitingForContinue = true;
        promptPanel?.SetActive(true);
        if (promptText != null) promptText.text = text;
    }

    void OnContinue()
    {
        waitingForContinue = false;
        promptPanel?.SetActive(false);
    }
}

// =========================
// DATA
// =========================
[System.Serializable]
public class TutorialStep
{
    [TextArea]
    public string promptText;           // shown in the prompt panel

    public bool enableInput      = false; // open player input during this step
    public bool keepInputAfter   = false; // don't re-block after condition met

    public TutorialWaitCondition waitCondition = TutorialWaitCondition.None;
}

public enum TutorialWaitCondition
{
    None,
    PlayerAssignedCard,
    PlayerConfirmedTurn,
    TurnResolved,
    EnemyDead
}