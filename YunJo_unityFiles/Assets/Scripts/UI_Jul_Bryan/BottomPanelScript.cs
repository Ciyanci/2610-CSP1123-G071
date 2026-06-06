using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections.Generic;

public class BottomPanelScript : MonoBehaviour
{
    public Transform contentTransform;   
    public Image previewImage;           
    public TextMeshProUGUI titleText;    
    public TextMeshProUGUI descriptionText; 

    private Dictionary<string, string> levelData = new Dictionary<string, string>()
    {
        { "Potato1", "Level 1: xxx." },
        { "Potato2", "Level 2: xxx." },
        { "Potato3", "Level 3: xxx." },
        { "Potato4", "Level 4: xxx." },
        { "Potato5", "Final 4: xxx." }
    };

    void Start()
    {
        SetupButtons();

        if (contentTransform.childCount > 0)
        {
            Transform firstChild = contentTransform.GetChild(0);
            Image firstImage = firstChild.GetComponent<Image>();
            
            if (firstImage != null)
            {
                UpdateDisplay(firstChild.name, firstImage.sprite);
            }
        }
    }

    void SetupButtons()
    {
        foreach (Transform child in contentTransform)
        {
            Button btn = child.GetComponent<Button>();
            Image btnImage = child.GetComponent<Image>();

            if (btn != null && btnImage != null)
            {
                string nameForEvent = child.name;
                Sprite spriteForEvent = btnImage.sprite;
                btn.onClick.AddListener(() => UpdateDisplay(nameForEvent, spriteForEvent));
            }
        }
    }

    public void UpdateDisplay(string itemName, Sprite itemSprite)
    {
        titleText.text = itemName;
        previewImage.sprite = itemSprite;

        if (levelData.ContainsKey(itemName))
        {
            descriptionText.text = levelData[itemName];
        }
        else
        {
            descriptionText.text = "No info available for " + itemName;
        }
    }

    public void EnterGameClick()
    {
        
    }
}