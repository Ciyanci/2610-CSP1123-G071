using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BottomBarController : MonoBehaviour
{
    public TextMeshProUGUI barText;
    public TextMeshProUGUI personNameText;
    public TextMeshProUGUI jobText;     // ← new

    private int sentenceIndex = -1;
    private StoryScene currentScene;
    private State state = State.COMPLETED;

    private enum State
    {
        PLAYING, COMPLETED
    }

    public void PlayScene(StoryScene scene)
    {
        currentScene = scene;
        sentenceIndex = -1;
        PlayNextSentence();
    }

    public void PlayNextSentence()
    {
        StartCoroutine(TypeText(currentScene.sentences[++sentenceIndex].text));

        var sentence = currentScene.sentences[sentenceIndex];

        Debug.Log("Speaker: " + sentence.speaker);

        if (sentence.speaker != null)
        {
            Debug.Log("Speaker Name: " + sentence.speaker.speakerName);
            personNameText.text = sentence.speaker.speakerName;
            personNameText.color = sentence.speaker.textColor;
        }
        else
        {
            Debug.LogWarning($"[BottomBar] Sentence {sentenceIndex} has no speaker assigned!");
            personNameText.text = "";
        }

        jobText.text = sentence.jobTitle;
    }

    public void SkipToEnd()
    {
        StopAllCoroutines();
        barText.text = currentScene.sentences[sentenceIndex].text;
        state = State.COMPLETED;
    }

    public bool IsCompleted()
    {
        return state == State.COMPLETED;
    }

    public bool IsLastSentence()
    {
        return sentenceIndex + 1 == currentScene.sentences.Count;
    }

    private IEnumerator TypeText(string text)
    {
        barText.text = "";
        state = State.PLAYING;
        int wordIndex = 0;

        while (state != State.COMPLETED)
        {
            barText.text += text[wordIndex];
            yield return new WaitForSeconds(0.05f);
            if(++wordIndex == text.Length)
            {
                state = State.COMPLETED;
                break;
            }
        }
    }

    public void ClearText()
    {
        if (barText != null) barText.text = "";
        if (personNameText != null) personNameText.text = "";
        if (jobText != null) jobText.text = "";
    }

    public int GetSentenceIndex() => sentenceIndex;
}