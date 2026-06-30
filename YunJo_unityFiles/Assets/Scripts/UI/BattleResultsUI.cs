using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BattleResultsUI : MonoBehaviour
{
    public static BattleResultsUI Instance;

    [Header("Panel")]
    public CanvasGroup panelGroup;
    public GameObject panel;

    [Header("Result Text")]
    public TextMeshProUGUI resultText;

    [Header("Result Colors")]
    public Color victoryColor = new Color(0.9f, 0.75f, 0.2f, 1f);
    public Color defeatColor  = new Color(0.7f, 0.15f, 0.15f, 1f);

    [Header("Timing")]
    public float fadeInDuration = 0.8f;
    public float autoExitDelay  = 6.0f;  // 0 = wait for click

    [Header("Exit")]
    public string exitSceneName = "MainMenu";

    bool shown    = false;
    bool canClick = false;
    bool playerWon = false;

    void Awake()
    {
        Instance = this;

        if (panelGroup != null) panelGroup.alpha = 0f;
        panel?.SetActive(false);
    }

    // =========================
    // ENTRY POINTS
    // =========================
    public void ShowVictory()
    {
        playerWon = true;
        Show("VICTORY", victoryColor);
    }
    public void ShowDefeat()
    {
        playerWon = false;
        Show("DEFEAT",  defeatColor);
    }

    // =========================
    // SHOW
    // =========================
    void Show(string result, Color color)
    {
        if (shown) return;
        shown = true;
        StartCoroutine(ShowSequence(result, color));
    }

    IEnumerator ShowSequence(string result, Color color)
    {
        // ✅ Hide all combat UI while screen is still black
        HideCombatUI();

        // ✅ Fade black screen back out
        yield return TurnTransitionUI.Instance?.FadeFromBlack();

        // Activate panel at zero alpha
        panel?.SetActive(true);
        if (panelGroup != null) panelGroup.alpha = 0f;

        if (resultText != null)
        {
            resultText.text  = result;
            resultText.color = color;
        }

        // Fade panel in
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            if (panelGroup != null)
                panelGroup.alpha = Mathf.Clamp01(t / fadeInDuration);
            yield return null;
        }

        if (panelGroup != null) panelGroup.alpha = 1f;

        canClick = true;

        if (autoExitDelay > 0f)
        {
            yield return new WaitForSeconds(autoExitDelay);
            Exit();
        }
    }

    // =========================
    // HIDE COMBAT UI
    // =========================
    void HideCombatUI()
    {
        HandUI.Instance?.Hide();
        CombatInfoBar.Instance?.Clear();
        CombatCardDisplayUI.Instance?.Hide();
        ArrowManager.Instance?.ClearAllArrows();

        if (UnitRegistry.Instance != null)
        {
            foreach (var u in UnitRegistry.Instance.players)
                u?.HideSpeed();
            foreach (var u in UnitRegistry.Instance.enemies)
                u?.HideSpeed();
        }
    }

    // =========================
    // INPUT
    // =========================
    void Update()
    {
        if (!canClick || !shown) return;
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
            Exit();
    }

    void Exit()
    {
        canClick = false;
        if (playerWon)
        {
            GameManager.Instance.CompleteCurrentScene();
        }
        if (!string.IsNullOrEmpty(exitSceneName))
            SceneManager.LoadScene(exitSceneName);
        else
            Debug.LogWarning("[RESULTS] exitSceneName not set in Inspector");
    }
}
