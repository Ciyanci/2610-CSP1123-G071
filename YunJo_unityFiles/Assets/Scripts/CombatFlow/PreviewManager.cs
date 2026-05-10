using UnityEngine;
using System.Collections.Generic;

public class PreviewManager : MonoBehaviour
{
    public static PreviewManager Instance;

    public ArrowController arrowPrefab;

    public List<PreviewIntent> previews = new();

    void Awake()
    {
        Instance = this;
    }

    public void QueuePreview(
        CharacterUnit user,
        CharacterUnit target,
        Card card)
    {
        if (user == null || target == null || card == null)
            return;

        ArrowController arrow =
            Instantiate(
                arrowPrefab,
                transform
            );

        arrow.Set(
            user.headAnchor,
            target.headAnchor
        );

        previews.Add(new PreviewIntent
        {
            user = user,
            target = target,
            card = card,
            arrow = arrow
        });

        Debug.Log($"[PREVIEW] {user.name} -> {target.name}");
    }

    public void HideAll()
    {
        foreach (var p in previews)
        {
            if (p.arrow != null)
                p.arrow.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        foreach (var p in previews)
        {
            if (p.arrow != null)
                Destroy(p.arrow.gameObject);
        }

        previews.Clear();
    }
}