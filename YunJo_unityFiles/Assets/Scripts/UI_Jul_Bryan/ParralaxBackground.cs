using UnityEngine;
using UnityEngine.UI;

public class ParralaxBackground : MonoBehaviour
{
    [Header("Settings")]
    public float moveStrength = 20f;
    public float smoothSpeed = 5f;

    private Vector3 startPos;
    private RectTransform rectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        float x = (mousePos.x / Screen.width - 0.5f)*2f;
        float y = (mousePos.y / Screen.height - 0.5f)*2f;
        Vector3 targetPos = startPos + new Vector3(x, y, 0f)*moveStrength;

        rectTransform.anchoredPosition = Vector3.Lerp
        (
            rectTransform.anchoredPosition,
            targetPos,
            Time.deltaTime*smoothSpeed
        );
    }
}
