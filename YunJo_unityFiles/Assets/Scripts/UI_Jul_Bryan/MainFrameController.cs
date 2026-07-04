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

    [Header("Visual Novel")]
    public LoadingBar loadingBar;
    public int visualNovelSceneId = 11;

    private bool isPanelOpen = false;
    private FrameButton currentActive = null;
    private bool hasSelection = false;

    void Start()
    {
        float panelWidth = infoPanel.rect.width;
        infoPanelHiddenPos = new Vector2(infoPanelVisiblePos.x + panelWidth + 20, infoPanelVisiblePos.y);
        infoPanel.anchoredPosition = infoPanelHiddenPos;
    }

    public void StartScene()
    {
        if (currentActive == null)
        {
            Debug.LogWarning("[MainFrame] No stage selected!");
            return;
        }

        if (currentActive.storyScene == null)
        {
            Debug.LogWarning($"[MainFrame] No StoryScene assigned to {currentActive.title}!");
            return;
        }

        // find LoadingBar automatically if not assigned
        if (loadingBar == null)
            loadingBar = FindObjectOfType<LoadingBar>(true); // true = include inactive

        if (loadingBar == null)
        {
            Debug.LogError("[MainFrame] LoadingBar not found in scene!");
            return;
        }

        Debug.Log($"[MainFrame] Starting scene: {currentActive.storyScene.name}");
        GameManager.Instance.SetTargetScene(currentActive.storyScene);
        loadingBar.LoadScene(visualNovelSceneId);
    }

    public void OnFrameButtonClicked(FrameButton button)
    {
        if (hasSelection && currentActive == button)
        {
            ClosePanel();
            return;
        }

        UpdateInfoPanel(button.title, button.description);

        if (!hasSelection)
        {
            hasSelection = true;
            currentActive = button;
            OpenPanel(button);
        }
        else
        {
            Vector2 targetPos = GetCenteredPos(button);
            currentActive = button;

            Sequence seq = DOTween.Sequence();
            seq.Append(mainFrame.DOAnchorPos(expandedPos, tweenDuration * 0.8f).SetEase(Ease.InQuart));
            seq.Join(mainFrame.DOSizeDelta(expandedSize, tweenDuration * 0.8f).SetEase(easeType));
            seq.Join(mainFrame.DOScale(Vector3.one, tweenDuration * 0.8f).SetEase(easeType));
            seq.AppendInterval(0.1f);
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
        Vector2 buttonWorld = buttonRect.position;
        Vector2 frameWorld = mainFrame.position;
        Vector2 diff = buttonWorld - frameWorld;

        Canvas canvas = mainFrame.GetComponentInParent<Canvas>();
        float scale = canvas.scaleFactor;
        Vector2 diffLocal = diff / scale;

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