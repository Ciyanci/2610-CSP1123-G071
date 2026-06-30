using UnityEngine;
using UnityEngine.UI;

public class LockButton : MonoBehaviour
{
    [Header("Button")]
    public Button continueButton;

    [Header("Required completed scene")]
    public string requiredSceneName = "VisualNovelTest";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshButton();
    }

    public void RefreshButton()
    {
        if (!SaveSystem.SaveExists())
        {
            continueButton.interactable = false;
        }
        GameManager.Instance.LoadGame();

        continueButton.interactable = GameManager.Instance.IsSceneCompleted(requiredSceneName);
    }
    
}
