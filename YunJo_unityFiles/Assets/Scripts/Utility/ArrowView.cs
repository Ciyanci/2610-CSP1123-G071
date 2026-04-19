using UnityEngine;
public class ArrowView : MonoBehaviour
{
    public Transform start;
    public Transform target;

    void Update()
    {
        if (!start || !target) return;

        Vector3 dir = (target.position - start.position).normalized;

        transform.position = start.position;
        transform.right = dir; // rotate arrow to face target
    }
}