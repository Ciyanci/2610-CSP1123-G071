using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public float factor = 0.1f;
    Vector3 start;

    void Start() => start = transform.position;

    void Update()
    {
        float offset = Mathf.Sin(Time.time) * factor;
        transform.position = start + new Vector3(offset, 0, 0);
    }
}
