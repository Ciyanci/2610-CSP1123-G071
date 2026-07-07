using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public StoryScene currentScene;
    public BottomBarController bottomBar;
    public BackgroundController backgroundController;
    public CharacterSpriteController characterController;
    public ChapterIntroController chapterIntro;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 0.3f;

    private bool isIntroPlaying = false;
    private bool canAdvance = false;
    private bool isFading = false;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.targetScene != null)
        {
            currentScene = GameManager.Instance.targetScene;
            GameManager.Instance.targetScene = null;
        }

        // start fully black
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }

        // clear placeholder text
        bottomBar.ClearText();

        PlayCurrentScene(alreadyBlack: true);
    }

    void PlayCurrentScene(bool alreadyBlack = false)
    {
        StartCoroutine(PlayCurrentSceneWithFade(alreadyBlack));
    }

    private IEnumerator PlayCurrentSceneWithFade(bool alreadyBlack = false)
    {
        isFading = true;

        if (!alreadyBlack)
            yield return StartCoroutine(Fade(0f, 1f));

        if (currentScene.hasChapterIntro)
        {
            isIntroPlaying = true;
            canAdvance = false;

            // set background and characters while black, hide bottombar
            backgroundController.SetImage(currentScene.backgroud);
            ShowCharacter(currentScene.sentences[0]);
            bottomBar.gameObject.SetActive(false);

            // don't hide fade image yet — let chapterIntro take over visibility
            // set fade image alpha to 0 but keep it active until overlay is shown
            if (fadeImage != null)
            {
                StartCoroutine(HandOffToChapterIntro());
            }
            isFading = false;

            chapterIntro.Show(currentScene.chapterNumber, currentScene.chapterName, () =>
            {
                isIntroPlaying = false;
                bottomBar.gameObject.SetActive(true);
                bottomBar.PlayScene(currentScene);
                StartCoroutine(AdvanceCooldown());
            });
        }
        else
        {
            canAdvance = false;

            backgroundController.SetImage(currentScene.backgroud);
            bottomBar.PlayScene(currentScene);
            ShowCharacter(currentScene.sentences[0]);

            yield return StartCoroutine(Fade(1f, 0f)); // fade in normally
            isFading = false;

            StartCoroutine(AdvanceCooldown());
        }
    }

    void GoToNextScene()
    {
        StartCoroutine(GoToNextSceneWithFade());
    }

    private IEnumerator GoToNextSceneWithFade()
    {
        isFading = true;
        yield return StartCoroutine(Fade(0f, 1f)); // fade to black

        if (currentScene.hasBattleScene)
        {
            SceneManager.LoadScene(currentScene.battleSceneName);
            yield break;
        }

        if (currentScene.loadMap)
        {
            SceneManager.LoadScene("LevelSelection");
            yield break;
        }

        if (currentScene.nextScene != null)
        {
            currentScene = currentScene.nextScene;
            PlayCurrentScene(alreadyBlack: true);
        }
        else
        {
            GameManager.Instance.CompleteCurrentScene();
            Debug.Log("End of story!");
        }

        isFading = false;
    }

    private IEnumerator Fade(float from, float to)
    {
        Debug.Log($"[Fade] Starting fade from {from} to {to}");
        if (fadeImage == null)
        {
            Debug.LogError("[Fade] fadeImage is NULL!");
            yield break;
        }

        fadeImage.gameObject.SetActive(true);
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;

        if (to == 0f)
            fadeImage.gameObject.SetActive(false);
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
            characterController.Show(sentence.characterSprite, sentence.characterPos);

        if (sentence.characterPos2 != StoryScene.CharacterPosition.None && sentence.characterSprite2 != null)
            characterController.Show(sentence.characterSprite2, sentence.characterPos2);

        if (sentence.characterPos3 != StoryScene.CharacterPosition.None && sentence.characterSprite3 != null)
            characterController.Show(sentence.characterSprite3, sentence.characterPos3);
    }

    public void SkipScene()
    {
        if (isIntroPlaying || isFading) return;

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
        if (!canAdvance || isIntroPlaying || isFading) return;

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

    private IEnumerator HandOffToChapterIntro()
    {
        // wait one frame for chapterIntro overlay to activate
        yield return null;
        // now safe to hide the fade image
        fadeImage.gameObject.SetActive(false);
    }
}