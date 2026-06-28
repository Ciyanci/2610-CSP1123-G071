using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Steps")]
    public List<TutorialStep> steps = new();

    //tutorial panel (blocks shit)
    [Header("Tutorial Panel")]
    public GameObject      tutorialPanel;
    public TextMeshProUGUI tutorialBodyText;
    public TextMeshProUGUI pageCounterText;
    public Button          nextPageButton;
    public TextMeshProUGUI nextPageButtonText;

    //hint bar (does not block shit)
    [Header("Hint Bar")]
    public GameObject      hintBar;
    public TextMeshProUGUI hintText;

    //internal state
    int          currentPage     = 0;
    List<string> activePages     = new();
    bool         waitingForPages = false;

    void Awake() => Instance = this;

    void Start()
    {
        CombatFlowController.Instance.SetInputEnabled(false);

        tutorialPanel?.SetActive(false);
        hintBar?.SetActive(false);

        nextPageButton?.onClick.AddListener(OnNextPageClicked);

        StartCoroutine(RunTutorial());
    }

    //main tutorial loop
    IEnumerator RunTutorial()
    {
        foreach (var step in steps)
        {
            //blocking panel first — player clicks through all pages
            if (step.tutorialPages != null && step.tutorialPages.Count > 0)
                yield return ShowTutorialPanel(step.tutorialPages);

            //hint bar — passive, just updates text (doesnt block shit)
            ShowHint(step.hintText);

            if (step.enableInput)
                CombatFlowController.Instance.SetInputEnabled(true);

            if (step.waitCondition != TutorialWaitCondition.None)
                yield return WaitForCondition(step.waitCondition);

            if (step.enableInput && !step.keepInputAfter)
                CombatFlowController.Instance.SetInputEnabled(false);
        }

        //all steps done
        HideHint();
        CombatFlowController.Instance.SetInputEnabled(true);
    }

    //tutorial panel
    IEnumerator ShowTutorialPanel(List<string> pages)
    {
        activePages      = pages;
        currentPage      = 0;
        waitingForPages  = true;

        tutorialPanel?.SetActive(true);
        RefreshPage();

        yield return new WaitUntil(() => !waitingForPages);
    }

    void RefreshPage()
    {
        if (tutorialBodyText != null)
            tutorialBodyText.text = activePages[currentPage];

        if (pageCounterText != null)
            pageCounterText.text  = activePages.Count > 1
                ? $"{currentPage + 1} / {activePages.Count}"
                : "";   // hide counter on single-page panels

        if (nextPageButtonText != null)
            nextPageButtonText.text = (currentPage == activePages.Count - 1)
                ? "OK"
                : "Next";
    }

    void OnNextPageClicked()
    {
        if (!waitingForPages) return;

        currentPage++;

        if (currentPage < activePages.Count)
        {
            RefreshPage();
        }
        else
        {
            waitingForPages = false;
            tutorialPanel?.SetActive(false);
        }
    }

    //hint bar
    void ShowHint(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            HideHint();
            return;
        }

        hintBar?.SetActive(true);
        if (hintText != null)
            hintText.text = text;
    }

    void HideHint()
    {
        hintBar?.SetActive(false);
    }

    //call this from anywhere to update the hint mid-step if needed
    public void SetHint(string text) => ShowHint(text);

    //wait for condition
    IEnumerator WaitForCondition(TutorialWaitCondition condition)
    {
        switch (condition)
        {
            case TutorialWaitCondition.PlayerAssignedCard:
                yield return new WaitUntil(() => AnyPlayerSlotPlanned());
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
}
