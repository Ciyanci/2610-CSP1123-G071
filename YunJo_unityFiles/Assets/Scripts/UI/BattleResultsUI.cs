using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
public class BattleResultsUI : MonoBehaviour
{
    public static BattleResultsUI Instance;
    public GameObject panel;
    public TextMeshProUGUI resultText;
    [Header("Exit Settings")]
    public string exitSceneName = "MainMenu"; // set in inspector
    public float  clickDelay    = 1.5f;       //min time before click registers
    public float  autoExitDelay = 0f;         //0 = no auto exit, >0 = auto exit after delay
    bool canClick  = false;
    bool shown     = false;
    void Awake()
    {
        Instance = this;
        panel?.SetActive(false);
    }
    public void ShowVictory() => Show("VICTORY");
    public void ShowDefeat()  => Show("DEFEAT");
    void Show(string msg)
    {
        if (shown) return;
        shown = true;
        panel?.SetActive(true);
        if (resultText != null) resultText.text = msg;
        StartCoroutine(EnableClickAfterDelay());
    }
    IEnumerator EnableClickAfterDelay()
    {
        yield return new WaitForSeconds(clickDelay);
        canClick = true;
        if (autoExitDelay > 0f)
        {
            yield return new WaitForSeconds(autoExitDelay);
            Exit();
        }
    }
    void Update()
    {
        if (!canClick) return;
        if (!shown)    return;
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
            Exit();
    }
    void Exit()
    {
        canClick = false;
        if (!string.IsNullOrEmpty(exitSceneName))
            SceneManager.LoadScene(exitSceneName);
        else
            Debug.LogWarning("[RESULTS] exitSceneName not set — assign in Inspector");
    }
}