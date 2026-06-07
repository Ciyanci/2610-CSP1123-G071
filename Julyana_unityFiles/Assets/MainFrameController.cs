using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainFrameController : MonoBehaviour
{
    [Header("Main Frame")]
    public RectTransform mainFrame;
    public Vector2 expandedSize = new Vector2(800, 600);
    public Vector2 collapsedSize = new Vector2(560, 600);
    public Vector2 expandedPos = Vector2.zero;
    public Vector2 collapsedPos = new Vector2(-120, 0); // shift left

    [Header("Info Panel")]
    public RectTransform infoPanel;
    public Vector2 infoPanelHiddenPos;   // set in Start(), off-screen right
    public Vector2 infoPanelVisiblePos;  // set in inspector or Start()
    public TMP_Text infoTitle;
    public TMP_Text infoBody;

    [Header("Animation")]
    public float tweenDuration = 0.4f;
    public Ease easeType = Ease.InOutQuart;

    private bool isPanelOpen = false;
    private FrameButton currentActive = null;

    void Start()
    {
        // Place info panel off-screen to the right initially
        float panelWidth = infoPanel.rect.width;
        infoPanelHiddenPos = new Vector2(infoPanelVisiblePos.x + panelWidth + 20, infoPanelVisiblePos.y);
        infoPanel.anchoredPosition = infoPanelHiddenPos;
    }

    // Called by each button when clicked
    public void OnFrameButtonClicked(FrameButton button)
    {
        // Same button clicked again — toggle close
        if (isPanelOpen && currentActive == button)
        {
            ClosePanel();
            return;
        }

        currentActive = button;

        // Update info panel content immediately (or after a slight delay if mid-transition)
        UpdateInfoPanel(button.title, button.description);

        if (!isPanelOpen)
        {
            OpenPanel();
        }
        else
        {
            // Already open — just refresh content with a quick punch scale
            infoPanel.DOKill();
            infoPanel.DOPunchScale(Vector3.one * 0.03f, 0.25f, 5, 0.5f);
        }
    }

    private void OpenPanel()
    {
        isPanelOpen = true;

        Sequence seq = DOTween.Sequence();

        // Simultaneously shrink + shift the main frame
        seq.Join(mainFrame.DOSizeDelta(collapsedSize, tweenDuration).SetEase(easeType));
        seq.Join(mainFrame.DOAnchorPos(collapsedPos, tweenDuration).SetEase(easeType));

        // Slide info panel in from the right
        seq.Join(infoPanel.DOAnchorPos(infoPanelVisiblePos, tweenDuration).SetEase(easeType));
    }

    private void ClosePanel()
    {
        isPanelOpen = false;
        currentActive = null;

        Sequence seq = DOTween.Sequence();

        // Restore main frame
        seq.Join(mainFrame.DOSizeDelta(expandedSize, tweenDuration).SetEase(easeType));
        seq.Join(mainFrame.DOAnchorPos(expandedPos, tweenDuration).SetEase(easeType));

        // Slide info panel back out
        seq.Join(infoPanel.DOAnchorPos(infoPanelHiddenPos, tweenDuration).SetEase(easeType));
    }

    private void UpdateInfoPanel(string title, string description)
    {
        if (infoTitle) infoTitle.text = title;
        if (infoBody)  infoBody.text  = description;
    }
}