using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PrepHighlightTutorialManager : MonoBehaviour
{
    [Header("Steps")]
    public List<PrepHighlightStep> steps = new();

    [Header("Overlay")]
    public GameObject overlayPanel;
    public Image darkBackground;
    public RectTransform highlightFrame;

    [Header("Tutorial Box")]
    public RectTransform tutorialBox;
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public Button nextButton;

    int currentStep = 0;

    void Start()
    {
        nextButton?.onClick.AddListener(NextStep);

        if (overlayPanel != null)
            overlayPanel.SetActive(false);

        StartTutorial();
    }

    public void StartTutorial()
    {
        if (steps.Count == 0) return;

        currentStep = 0;

        overlayPanel.SetActive(true);
        overlayPanel.transform.SetAsLastSibling();

        ShowStep();
    }

    void ShowStep()
    {
        PrepHighlightStep step = steps[currentStep];

        if (titleText != null)
            titleText.text = step.title;

        if (bodyText != null)
            bodyText.text = step.description;

        if (step.targetUI != null && highlightFrame != null)
        {
            highlightFrame.gameObject.SetActive(true);
            highlightFrame.position = step.targetUI.position;
            highlightFrame.sizeDelta = step.targetUI.rect.size + step.highlightPadding;
        }
        else if (highlightFrame != null)
        {
            highlightFrame.gameObject.SetActive(false);
        }

        if (step.boxPosition != null && tutorialBox != null)
        {
            tutorialBox.position =
                (Vector2)step.boxPosition.position + step.boxPadding;
        }
    }

    void NextStep()
    {
        currentStep++;

        if (currentStep >= steps.Count)
        {
            overlayPanel.SetActive(false);
            return;
        }

        ShowStep();
    }
}

[System.Serializable]
public class PrepHighlightStep
{
    [Header("Text")]
    public string title;

    [TextArea]
    public string description;

    [Header("Highlight")]
    public RectTransform targetUI;
    public Vector2 highlightPadding = new Vector2(20f, 20f);

    [Header("Tutorial Box Optional Position")]
    public RectTransform boxPosition;
    public Vector2 boxPadding = Vector2.zero;
}