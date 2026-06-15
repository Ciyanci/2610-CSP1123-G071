using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassiveEntryUI : MonoBehaviour
{
    public Image           icon;
    public TextMeshProUGUI passiveNameText;
    public TextMeshProUGUI descriptionText;

    public void Setup(PassiveData passive)
    {
        if (icon             != null && passive.icon != null) icon.sprite = passive.icon;
        if (passiveNameText  != null) passiveNameText.text  = passive.passiveName;
        if (descriptionText  != null) descriptionText.text  = passive.description;
    }
}
