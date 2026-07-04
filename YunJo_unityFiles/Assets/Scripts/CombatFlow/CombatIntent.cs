using UnityEngine;

[System.Serializable]
public class CombatIntent
{
    public CharacterUnit user;
    public CharacterUnit target;
    public SpeedSlot speedSlot;
    public Card card;
    public int priority;

    CombatPageRuntime runtimePage;

    public bool IsValid =>
        user   != null &&
        target != null &&
        card   != null &&
        !user.IsDead   &&
        !target.IsDead;

    //creates once, returns cached after that
    public CombatPageRuntime GetOrCreatePage()
    {
        if (runtimePage == null)
        {
            runtimePage = new CombatPageRuntime(user, target, card);
            Debug.Log($"[PAGE] Created page {runtimePage.PageId}: {user.unitName} to {target.unitName}");
        }
        return runtimePage;
    }

    public void ClearPage()
    {
        runtimePage = null;
    }
}
