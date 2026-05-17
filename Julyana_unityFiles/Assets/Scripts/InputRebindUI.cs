using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class InputRebindUI : MonoBehaviour
{
    [Header("Input Action")]
    public InputActionReference actionReference;

    [Header("Binding Index (0 usually main key)")]
    public int bindingIndex = 0;

    [Header("UI")]
    public TMP_Text bindingText;
    public Button rebindButton;
    public TMP_Text rebindButtonText;

    private const string SAVE_KEY = "rebinds";

    private void Awake()
    {
        LoadBindings();
    }

    private void Start()
    {
        LoadBindings();
        UpdateBindingText();
        rebindButton.onClick.AddListener(StartRebind);
    }

    void StartRebind()
    {
        rebindButton.interactable = false;
        rebindButtonText.text = "Press a key...";

        actionReference.action.Disable();

        actionReference.action.PerformInteractiveRebinding(bindingIndex).WithCancelingThrough("<Keyboard>/escape").OnComplete(operation =>
        {
            operation.Dispose();
            actionReference.action.Enable();
            rebindButtonText.text = "Rebind";

            SaveBindings();
            UpdateBindingText();
        })
        .Start();
    }

    void UpdateBindingText()
        {
            bindingText.text = InputControlPath.ToHumanReadableString(
                actionReference.action.bindings[bindingIndex].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);
        }

        void SaveBindings()
        {
            PlayerPrefs.SetString(SAVE_KEY, actionReference.action.actionMap.asset.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        }

        void LoadBindings()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                actionReference.action.actionMap.asset.LoadBindingOverridesFromJson(json);
            }
        }
}
