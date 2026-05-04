using UnityEngine;

public class CombatInputController : MonoBehaviour
{
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!CombatFlowController.Instance.inputEnabled)
            return;

        // LEFT CLICK → select unit
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0;

            Collider2D hit = Physics2D.OverlapPoint(mouse);

            if (hit == null) return;

            CharacterUnit unit = hit.GetComponent<CharacterUnit>();

            if (unit != null)
            {
                CombatFlowController.Instance.SelectUnit(unit);
            }
        }

        // RIGHT CLICK → cancel selection
        if (Input.GetMouseButtonDown(1))
        {
            CombatFlowController.Instance.ResetAll();
        }
    }
}