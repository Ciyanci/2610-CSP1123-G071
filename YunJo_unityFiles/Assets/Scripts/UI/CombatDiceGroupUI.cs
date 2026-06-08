using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CombatDiceGroupUI : MonoBehaviour
{
    [Header("Card Display")]
    public Image cardArtwork;
    public TextMeshProUGUI cardNameText;

    [Header("Dice Display — single active die")]
    public CombatDiceElement activeDiceElement;  //one die shown at a time

    [Header("Attack Type Icons — all shown simultaneously")]
    public Transform typeIconContainer; 
    public DiceTypeIconUI typeIconPrefab; 

    [Header("Follow")]
    public Vector3 worldOffset = new Vector3(0f, 2.2f, 0f);

    [Header("Fade")]
    public CanvasGroup canvasGroup;

    //runtime stuff
    Transform followTarget;
    Camera cam;
    RectTransform rect;

    int currentDieIndex = 0;
    List<DiceData> currentDice = new();
    List<DiceTypeIconUI> spawnedIcons = new();

    void Awake()
    {
        cam  = Camera.main;
        rect = GetComponent<RectTransform>();
        Hide();
    }

    void LateUpdate()
    {
        if (followTarget == null || cam == null) return;

        Vector3 screen = cam.WorldToScreenPoint(
            followTarget.position + worldOffset);

        if (screen.z <= 0) return;
        rect.position = screen;
    }

    //bind (called once per action)
    public void Bind(CharacterUnit unit, Card card)
    {
        followTarget    = unit.headAnchor != null ? unit.headAnchor : unit.transform;
        currentDieIndex = 0;
        currentDice     = card.GetDice();

        //card
        if (cardArtwork  != null) cardArtwork.sprite = card.Artwork;
        if (cardNameText != null) cardNameText.text  = card.Name;

        //spawn small icon based on dice
        ClearTypeIcons();
        foreach (var die in currentDice)
        {
            var icon = Instantiate(typeIconPrefab, typeIconContainer);
            icon.Setup(die);
            spawnedIcons.Add(icon);
        }

        //prime first dice
        RefreshActiveDie();

        Show();
    }

    //roll current die (roll anim, data is called back)
    public IEnumerator RollCurrentDie(int min, int max, System.Action<int> onDone)
    {
        if (activeDiceElement == null)
        {
            onDone?.Invoke(Random.Range(min, max + 1));
            yield break;
        }
        int result = Random.Range(min, max + 1);
        float t    = 0f;
        float dur  = 0.45f;
        //animate random numbers — unit plays windup DURING this window
        CombatAudioManager.Instance?.PlayDiceRoll();
        while (t < dur)
        {
            activeDiceElement.SetValue(Random.Range(min, max + 1));
            t += Time.deltaTime;
            yield return null;
        }
        //lock the final value visually
        activeDiceElement.SetValue(result);
        //fire callback timing
        yield return new WaitForSeconds(0.3f);
        onDone?.Invoke(result);
    }

    //result (colours the dice)
    public void SetCurrentResult(bool won)
    {
        activeDiceElement?.SetResult(won);
    }

    // break (loser die, play break anim haha skill issue)
    public void BreakCurrentDie()
    {
        activeDiceElement?.Break();
        if (currentDieIndex < spawnedIcons.Count)
            spawnedIcons[currentDieIndex].SetSpent();
    }
    //advance (move to next die in sequence)
    public void AdvanceDie()
    {
        if (currentDieIndex < spawnedIcons.Count)
            spawnedIcons[currentDieIndex].SetHighlight(false);
        currentDieIndex++;
        RefreshActiveDie();
    }

    //visibilly
    public void Show()
    {
        gameObject.SetActive(true);
        if (canvasGroup != null) canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    //helper

    //refreshes the single active die
    void RefreshActiveDie()
    {
        if (activeDiceElement == null) return;
        if (currentDieIndex < currentDice.Count)
        {
            //makes sure that it always force-enable and reset before setup
            //this prevents BreakAnim coroutine finishing late and hiding the new die (nevermind it doesnt wtf)
            activeDiceElement.gameObject.SetActive(true);
            activeDiceElement.StopAllCoroutines();
            activeDiceElement.Setup(currentDice[currentDieIndex]);
            for (int i = 0; i < spawnedIcons.Count; i++)
                spawnedIcons[i].SetHighlight(i == currentDieIndex);
        }
        else
        {
            activeDiceElement.gameObject.SetActive(false);
            foreach (var icon in spawnedIcons)
                icon.SetHighlight(false);
        }
    }

    void ClearTypeIcons()
    {
        foreach (var icon in spawnedIcons)
            if (icon != null) Destroy(icon.gameObject);
        spawnedIcons.Clear();
    }
}
