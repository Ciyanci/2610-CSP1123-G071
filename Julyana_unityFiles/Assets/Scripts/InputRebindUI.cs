using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputRebindUI : MonoBehaviour
{
    [Header("Input Action")]
    public InputActionReference actionReference;

    [Header("Binding Index")]
    public int bindingIndex = 0;

    [Header("UI")]
    public TMP_Text bindingText;
    public Button rebindButton;
    public TMP_Text rebindButtonText;

    private const string SAVE_KEY = "rebinds";

    private void Start()
    {
        if (actionReference == null || actionReference.action == null)
        {
            Debug.LogError("InputActionReference not assigned!");
            return;
        }

        LoadBindings();
        UpdateBindingText();

        rebindButton.onClick.AddListener(StartRebind);
        rebindButtonText.text = "Rebind";
    }

    void StartRebind()
    {
        // IMPORTANT: prevents UI from blocking Enter / keyboard input
        EventSystem.current.SetSelectedGameObject(null);

        var action = actionReference.action;

        rebindButton.interactable = false;
        rebindButtonText.text = "Press a key...";

        action.Disable();

        action.PerformInteractiveRebinding(bindingIndex)

            // Allows Enter + Numpad Enter to be detected properly
            .WithControlsHavingToMatchPath("<Keyboard>")

            // Prevent mouse clicks from interfering
            .WithControlsExcluding("<Mouse>")

            // ESC cancels rebinding
            .WithCancelingThrough("<Keyboard>/escape")

            .OnComplete(operation =>
            {
                operation.Dispose();
                action.Enable();

                rebindButton.interactable = true;
                rebindButtonText.text = "Rebind";

                SaveBindings();
                UpdateBindingText();
            })

            .OnCancel(operation =>
            {
                operation.Dispose();
                action.Enable();

                rebindButton.interactable = true;
                rebindButtonText.text = "Rebind";
            })

            .Start();
    }

    void UpdateBindingText()
    {
        var action = actionReference.action;

        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
        {
            bindingText.text = "Invalid Binding Index";
            return;
        }

        bindingText.text = InputControlPath.ToHumanReadableString(
            action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );
    }

    void SaveBindings()
    {
        string json = actionReference.action.actionMap.asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    void LoadBindings()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY)) return;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        actionReference.action.actionMap.asset.LoadBindingOverridesFromJson(json);
    }
}
