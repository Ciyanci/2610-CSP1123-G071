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
        Vector3 end = start + Vector3.up * 60f;   //UI pixels

        while (t < duration)
        {
            float p = t / duration;

            transform.position = Vector3.Lerp(start, end, p);

            //fade out
            Color c = text.color;
            c.a = 1f - p;
            text.color = c;

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}