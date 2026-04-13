using UnityEngine;

public class UIManager : MonoBehaviour
{
        public static UIManager uIManager { get; private set;}

    void Awake()
    {
        if (uIManager != null && uIManager != this)
        {
            Destroy(this);
        }
        else
        {
            uIManager = this;
        }
    }
}
