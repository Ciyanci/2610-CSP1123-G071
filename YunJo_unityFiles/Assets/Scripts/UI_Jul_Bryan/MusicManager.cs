using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    [Header("References")]
    public AudioSource bgm;

    [Header("StopBgm In These Scenes")]
    public int[] silentSceneIndexes;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

     void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var index in silentSceneIndexes)
        {
            if (scene.buildIndex == index)
            {
                bgm.Stop();
                return;
            }
        }
        if (!bgm.isPlaying)
        bgm.Play();
    }
}
