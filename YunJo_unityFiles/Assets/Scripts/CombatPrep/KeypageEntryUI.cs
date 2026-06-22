using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeypageEntryUI : MonoBehaviour
{
    public Image           art;
    public TextMeshProUGUI keypageNameText;
    public TextMeshProUGUI statsText;
    public Image           equippedHighlight;
    public Button          equipButton;

    KeypageData   boundKeypage;
    CharacterUnit boundUnit;

    public void Setup(KeypageData keypage, bool isEquipped, CharacterUnit unit)
    {
        boundKeypage = keypage;
        boundUnit    = unit;

        if (art             != null && keypage.art != null) art.sprite = keypage.art;
        if (keypageNameText != null) keypageNameText.text = keypage.keypageName;

        if (statsText != null)
        {
            string hp  = keypage.hpBonus      != 0 ? $"HP {keypage.hpBonus:+#;-#;0}"      : "";
            string stg = keypage.staggerBonus != 0 ? $"  STG {keypage.staggerBonus:+#;-#;0}" : "";
            statsText.text = $"{hp}{stg}".Trim();
        }

        equippedHighlight?.gameObject.SetActive(isEquipped);

        equipButton?.onClick.RemoveAllListeners();
        equipButton?.onClick.AddListener(() =>
            CombatPrepManager.Instance?.EquipKeypage(boundUnit, boundKeypage));
    }
}
