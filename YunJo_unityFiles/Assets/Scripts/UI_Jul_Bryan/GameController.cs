using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public StoryScene currentScene;
    public BottomBarController bottomBar;
    public BackgroundController backgroundController;
    public CharacterSpriteController characterController;
    public ChapterIntroController chapterIntro;

    private bool isIntroPlaying = false;  // ← new
    private bool canAdvance = false;      // ← new

    void Start()
    {
        PlayCurrentScene();
    }

    void PlayCurrentScene()
    {
        if (currentScene.hasChapterIntro)
        {
            isIntroPlaying = true;
            canAdvance = false;
            chapterIntro.Show(currentScene.chapterNumber, currentScene.chapterName, () =>
            {
                isIntroPlaying = false;
                bottomBar.PlayScene(currentScene);
                backgroundController.SetImage(currentScene.backgroud);
                ShowCharacter(currentScene.sentences[0]);
                StartCoroutine(AdvanceCooldown()); // ← start cooldown after intro
            });
        }
        else
        {
            canAdvance = false;
            bottomBar.PlayScene(currentScene);
            backgroundController.SetImage(currentScene.backgroud);
            ShowCharacter(currentScene.sentences[0]);
            StartCoroutine(AdvanceCooldown()); // ← start cooldown on scene start
        }
    }

    private IEnumerator AdvanceCooldown()
    {
        canAdvance = false;
        yield return new WaitForSeconds(0.5f);
        canAdvance = true;
    }

    void ShowCharacter(StoryScene.Sentence sentence)
    {
        string sprite1Name = sentence.characterSprite != null ? sentence.characterSprite.name : "NULL";
        string sprite2Name = sentence.characterSprite2 != null ? sentence.characterSprite2.name : "NULL";
        string sprite3Name = sentence.characterSprite3 != null ? sentence.characterSprite3.name : "NULL";
        Debug.Log($"[GameController] Sentence data — Sprite1: {sprite1Name}, Pos1: {sentence.characterPos}, Sprite2: {sprite2Name}, Pos2: {sentence.characterPos2}, Sprite3: {sprite3Name}, Pos3: {sentence.characterPos3}");

        characterController.Hide();

        if (sentence.characterPos != StoryScene.CharacterPosition.None && sentence.characterSprite != null)
        {
            characterController.Show(sentence.characterSprite, sentence.characterPos);
        }

        if (sentence.characterPos2 != StoryScene.CharacterPosition.None && sentence.characterSprite2 != null)
        {
            characterController.Show(sentence.characterSprite2, sentence.characterPos2);
        }

        if (sentence.characterPos3 != StoryScene.CharacterPosition.None && sentence.characterSprite3 != null)
        {
            characterController.Show(sentence.characterSprite3, sentence.characterPos3);
        }
    }

    public void SkipScene()
    {
        if (isIntroPlaying) return; // ← block skip during intro

        if (currentScene.nextScene != null)
        {
            currentScene = currentScene.nextScene;

            while (!currentScene.hasChapterIntro && currentScene.nextScene != null)
            {
                currentScene = currentScene.nextScene;
            }

            backgroundController.SwitchImage(currentScene.backgroud);
            PlayCurrentScene();
        }
        else
        {
            GameManager.Instance.CompleteCurrentScene();
            Debug.Log("End of story!");
        }
    }

    void Update()
    {
        if (!canAdvance || isIntroPlaying) return; // ← block all input during intro or cooldown

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (bottomBar.IsCompleted())
            {
                if (bottomBar.IsLastSentence())
                {
                    if (currentScene.nextScene != null)
                    {
                        currentScene = currentScene.nextScene;
                        backgroundController.SwitchImage(currentScene.backgroud);
                        PlayCurrentScene();
                    }
                    else
                    {
                        GameManager.Instance.CompleteCurrentScene();
                        Debug.Log("End of story!");
                    }
                }
                else
                {
                    ShowCharacter(currentScene.sentences[bottomBar.GetSentenceIndex() + 1]);
                    bottomBar.PlayNextSentence();
                    StartCoroutine(AdvanceCooldown()); // ← cooldown after each advance
                }
            }
            else
            {
                bottomBar.SkipToEnd();
            }
        }
    }
}