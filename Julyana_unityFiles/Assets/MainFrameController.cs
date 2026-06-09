using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using JetBrains.Annotations;

public class MainFrameController : MonoBehaviour
{
    [Header("Main Frame")]
    public RectTransform mainFrame;
    public Vector2 expandedSize = new Vector2(800, 600);
    public Vector2 collapsedSize = new Vector2(560, 600);
    public Vector2 expandedPos = Vector2.zero;

    [Header("Info Panel")]
    public RectTransform infoPanel;
    public Vector2 infoPanelHiddenPos;
    public Vector2 infoPanelVisiblePos;
    public TMP_Text infoTitle;
    public TMP_Text infoBody;

    [Header("Animation")]
    public float tweenDuration = 0.4f;
    public Ease easeType = Ease.InOutQuart;
    public Vector3 zoomedScale = new Vector3(1.3f, 1.3f, 1f);

    private bool isPanelOpen = false;
    private FrameButton currentActive = null;
    private bool hasSelection = false;

    void Start()
    {
        float panelWidth = infoPanel.rect.width;
        infoPanelHiddenPos = new Vector2(infoPanelVisiblePos.x + panelWidth + 20, infoPanelVisiblePos.y);
        infoPanel.anchoredPosition = infoPanelHiddenPos;
    }

    public void OnFrameButtonClicked(FrameButton button)
    {
        // Same button while selected: close and reset
        if (hasSelection && currentActive == button)
        {
            ClosePanel();
            return;
        }

        UpdateInfoPanel(button.title, button.description);

        if (!hasSelection)
        {
            // First ever press
            hasSelection = true;
            currentActive = button;
            OpenPanel(button);
        }
        else
        {
            // Capture position NOW while frame is at rest
            Vector2 targetPos = GetCenteredPos(button);
            currentActive = button;

            Sequence seq = DOTween.Sequence();

            // Step 1: snap back to center AND restore original size/scale simultaneously
            seq.Append(mainFrame.DOAnchorPos(expandedPos, tweenDuration * 0.8f).SetEase(Ease.InQuart));
            seq.Join(mainFrame.DOSizeDelta(expandedSize, tweenDuration * 0.8f).SetEase(easeType));
            seq.Join(mainFrame.DOScale(Vector3.one, tweenDuration * 0.8f).SetEase(easeType));

            // Step 2: hold at original position for tweenDuration
            seq.AppendInterval(0.1f);

            // Step 3: move to new button
            seq.Append(mainFrame.DOAnchorPos(targetPos, tweenDuration * 0.8f).SetEase(Ease.OutQuart));
            seq.Join(mainFrame.DOSizeDelta(collapsedSize, tweenDuration * 0.8f).SetEase(easeType));
            seq.Join(mainFrame.DOScale(zoomedScale, tweenDuration * 0.8f).SetEase(easeType));

            infoPanel.DOKill();
            infoPanel.DOPunchScale(Vector3.one * 0.03f, 0.25f, 5, 0.5f);
        }
    }

    private void ClosePanel()
    {
        hasSelection = false;
        currentActive = null;
        isPanelOpen = false;

        Sequence seq = DOTween.Sequence();
        seq.Join(mainFrame.DOSizeDelta(expandedSize, tweenDuration).SetEase(easeType));
        seq.Join(mainFrame.DOAnchorPos(expandedPos, tweenDuration).SetEase(easeType));
        seq.Join(mainFrame.DOScale(Vector3.one, tweenDuration).SetEase(easeType));
        seq.Join(infoPanel.DOAnchorPos(infoPanelHiddenPos, tweenDuration).SetEase(easeType));
    }

    private void OpenPanel(FrameButton button)
    {
        isPanelOpen = true;

        // Capture before any tween touches the frame
        Vector2 targetPos = GetCenteredPos(button);

        Sequence seq = DOTween.Sequence();
        seq.Join(mainFrame.DOSizeDelta(collapsedSize, tweenDuration).SetEase(easeType));
        seq.Join(mainFrame.DOAnchorPos(targetPos, tweenDuration).SetEase(easeType));
        seq.Join(mainFrame.DOScale(zoomedScale, tweenDuration).SetEase(easeType));
        seq.Join(infoPanel.DOAnchorPos(infoPanelVisiblePos, tweenDuration).SetEase(easeType));
    }

    private Vector2 GetCenteredPos(FrameButton button)
    {
        RectTransform buttonRect = button.GetComponent<RectTransform>();

        // Get both positions in canvas/root space, unaffected by current animation state
        Vector2 buttonWorld = buttonRect.position;
        Vector2 frameWorld = mainFrame.position;

        // How far is the button from the frame's current pivot in world units
        Vector2 diff = buttonWorld - frameWorld;

        // Convert world unit difference to local canvas units using the canvas scale
        Canvas canvas = mainFrame.GetComponentInParent<Canvas>();
        float scale = canvas.scaleFactor;
        Vector2 diffLocal = diff / scale;

        // Sidebar offset: shift left so button sits in the visible portion
        float sidebarWidth = 300;
        float sidebarOffset = sidebarWidth / 2f;

        return new Vector2(expandedPos.x - diffLocal.x - sidebarOffset, expandedPos.y - diffLocal.y);
    }

    private void UpdateInfoPanel(string title, string description)
    {
        if (infoTitle) infoTitle.text = title;
        if (infoBody)  infoBody.text  = description;
    }
}