using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMPStartButtonGlow : MonoBehaviour
{
    public TextMeshProUGUI Subtext;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void MouseInColor()
    {
        Subtext.color = Color.green;
    }

    // Update is called once per frame
    public void MouseOutColor()
    {
        Subtext.color = Color.white;
    }
}
