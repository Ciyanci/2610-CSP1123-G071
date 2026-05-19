using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    public bool isSwitched = false;
    private Image background1;
    private Image background2;
    public Animator animator;

    void Awake()
    {
        Image[] images = GetComponentsInChildren<Image>();
        background1 = images[0];
        background2 = images[1];
    }

    public void SetImage(Sprite sprite)
    {
        background1.sprite = sprite;
    }

    public void SwitchImage(Sprite sprite)
    {
        if (!isSwitched)
        {
            background2.sprite = sprite;
            animator.SetTrigger("SwitchFirst");
        }
        else
        {
            background1.sprite = sprite;
            animator.SetTrigger("SwitchSecond");
        }
        isSwitched = !isSwitched;
    }
}