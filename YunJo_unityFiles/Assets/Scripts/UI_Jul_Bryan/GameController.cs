using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public StoryScene currentScene;
    public BottomBarController bottomBar;
    public BackgroundController backgroundController;
    public CharacterSpriteController characterController;
    public ChapterIntroController chapterIntro;

    private bool isIntroPlaying = false;
    private bool canAdvance = false;

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
                StartStoryScene();
            });
        }
        else
        {
            StartStoryScene();
        }
    }

    void StartStoryScene()
    {
        canAdvance = false;

        bottomBar.PlayScene(currentScene);
        backgroundController.SetImage(currentScene.backgroud);

        if (currentScene.sentences.Count > 0)
        {
            ShowCharacter(currentScene.sentences[0]);
        }

        StartCoroutine(AdvanceCooldown());
    }

    private IEnumerator AdvanceCooldown()
    {
        canAdvance = false;
        yield return new WaitForSeconds(0.5f);
        canAdvance = true;
    }

    void ShowCharacter(StoryScene.Sentence sentence)
    {
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

        GoToNextScene();
    }

    void Update()
    {
        if (!canAdvance || isIntroPlaying) return;

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
                    StartCoroutine(AdvanceCooldown());
                }
            }
            else
            {
                bottomBar.SkipToEnd();
            }
        }
    }
}