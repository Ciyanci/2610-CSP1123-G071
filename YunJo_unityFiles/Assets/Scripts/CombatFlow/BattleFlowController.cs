using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFlowController : MonoBehaviour
{
    public static BattleFlowController Instance;

    public CombatResolver resolver;

    public bool inputEnabled;

    public List<CombatIntent> previewIntents = new();

    void Awake()
    {
        Instance = this;
    }

    public void SetInputEnabled(bool value)
    {
        inputEnabled = value;
    }

    public void QueuePreview(CharacterUnit user, CharacterUnit target, Card card)
    {
        previewIntents.Add(new CombatIntent
        {
            user = user,
            target = target,
            card = card,
            priority = card.Cost
        });
    }

    public void ClearPreview()
    {
        previewIntents.Clear();
    }

    // 🔥 FIX: replaces missing ResolveAll usage
    public IEnumerator ResolveTurn()
    {
        var intents = IntentBuilder.Build(PreviewManager.Instance.previews);
        var clashes = ClashDetector.Build(intents);

        yield return resolver.Resolve(new CombatTurnContext
        {
            intents = intents,
            clashes = clashes
        });

        ClearPreview();
    }
}