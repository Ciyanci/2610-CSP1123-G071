using UnityEngine;

public class MenuToggleButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator menuAnimator;
    private bool isOpen = false;

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        menuAnimator.SetBool("isOpen", isOpen);
    }
}