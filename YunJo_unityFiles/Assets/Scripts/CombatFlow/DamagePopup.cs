using UnityEngine;
using TMPro;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float riseSpeed = 2f;
    public float duration = 0.6f;

    public void Show(int damage)
    {
        text.text = damage.ToString();
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float t = 0f;
        Vector3 start = transform.position;

        while (t < duration)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}