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

    [Header("Tutorial Panel — blocking, image-based")]
    public GameObject      tutorialPanel;
    public Image           tutorialImage;
    public TextMeshProUGUI pageCounterText;
    public Button          nextPageButton;
    public TextMeshProUGUI nextPageButtonText;
    public Button          prevPageButton;

    [Header("Hint Bar — non-blocking")]
    public GameObject      hintBar;
    public TextMeshProUGUI hintText;

    int          currentPage    = 0;
    List<Sprite> activePages    = new();
    bool         waitingForPages = false;

    void Awake() => Instance = this;

    void Start()
    {
        if (CombatStateMachine.Instance != null)
            CombatStateMachine.Instance.tutorialControllingInput = true;

        CombatFlowController.Instance.SetInputEnabled(false);

        tutorialPanel?.SetActive(false);
        hintBar?.SetActive(false);

        nextPageButton?.onClick.AddListener(OnNextClicked);
        prevPageButton?.onClick.AddListener(OnPrevClicked);

        StartCoroutine(RunTutorial());
    }

    //main loop
    IEnumerator RunTutorial()
    {
        foreach (var step in steps)
        {
            //blocking image panel first
            if (step.tutorialImages != null && step.tutorialImages.Count > 0)
                yield return ShowTutorialPanel(step.tutorialImages);
            //hint bar — passive
            ShowHint(step.hintText);
            if (step.enableInput)
            {
                CombatStateMachine.Instance.tutorialControllingInput = false;
                CombatFlowController.Instance.SetInputEnabled(true);
            }
            if (step.waitCondition != TutorialWaitCondition.None)
                yield return WaitForCondition(step.waitCondition);
            if (step.enableInput && !step.keepInputAfter)
            {
                CombatFlowController.Instance.SetInputEnabled(false);
                CombatStateMachine.Instance.tutorialControllingInput = true;
            }
        }
        //all steps done — ensure full release
        HideHint();
        CombatStateMachine.Instance.tutorialControllingInput = false;
        CombatFlowController.Instance.SetInputEnabled(true);
    }

    //tutorial panel
    IEnumerator ShowTutorialPanel(List<Sprite> images)
    {
        activePages     = images;
        currentPage     = 0;
        waitingForPages = true;

        tutorialPanel?.SetActive(true);
        RefreshPage();

        yield return new WaitUntil(() => !waitingForPages);
    }

    void RefreshPage()
    {
        //show image for current page
        if (tutorialImage != null && currentPage < activePages.Count)
            tutorialImage.sprite = activePages[currentPage];

        //page counter
        if (pageCounterText != null)
            pageCounterText.text = activePages.Count > 1
                ? $"{currentPage + 1} / {activePages.Count}"
                : "";

        //next/OK label
        if (nextPageButtonText != null)
            nextPageButtonText.text = (currentPage == activePages.Count - 1)
                ? "OK" : "Next";

        //hide back button on first page
        prevPageButton?.gameObject.SetActive(currentPage > 0);
    }

    void OnNextClicked()
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

    void OnPrevClicked()
    {
        if (!waitingForPages || currentPage <= 0) return;
        currentPage--;
        RefreshPage();
    }

    //hint
    void ShowHint(string text)
    {
        if (string.IsNullOrEmpty(text)) { HideHint(); return; }
        hintBar?.SetActive(true);
        if (hintText != null) hintText.text = text;
    }

    void HideHint() => hintBar?.SetActive(false);

    public void SetHint(string text) => ShowHint(text);

    //wait cons
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

//data
[System.Serializable]
public class TutorialStep
{
    [Header("Tutorial Panel — images shown before this step")]
    public List<Sprite> tutorialImages = new();

    [Header("Hint Bar — shown during this step")]
    public string hintText = "";

    [Header("Gameplay")]
    public bool enableInput    = false;
    public bool keepInputAfter = false;
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
