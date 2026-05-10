using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    public static TargetSelector Instance;

    public CharacterUnit currentTarget;

    void Awake()
    {
        Instance = this;
    }

    public void SelectTarget(CharacterUnit target)
    {
        currentTarget = target;
        Debug.Log($"[TARGET] Selected {target.unitName}");
    }

    public CharacterUnit GetTarget()
    {
        return currentTarget;
    }

    public void Clear()
    {
        currentTarget = null;
    }
}