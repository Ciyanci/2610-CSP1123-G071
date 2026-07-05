using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//background variable must be renamed to backgroud cuz spelling mistake in StoryScene.cs whoops
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
        if (GameManager.Instance != null && GameManager.Instance.targetScene != null)
        {
            currentScene = GameManager.Instance.targetScene;
            GameManager.Instance.targetScene = null; // clear after use
        }
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

    void GoToNextScene()
    {
        if (currentScene.hasBattleScene)
        {
            SceneManager.LoadScene(currentScene.battleSceneName);
            return;
        }

        if (currentScene.loadMap)
        {
            SceneManager.LoadScene("LevelSelection");
        }

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

   public void SkipScene()
    {
        if (isIntroPlaying) return;

        if (currentScene.hasBattleScene)
        {
            SceneManager.LoadScene(currentScene.battleSceneName);
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("[GameController] GameManager is NULL — loading level selection directly");
            FindObjectOfType<LoadingBar>(true).LoadScene(11);
            return;
        }


        GameManager.Instance.CompleteCurrentScene();
        GameManager.Instance.ReturnToLevelSelection();
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
                    GoToNextScene();
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