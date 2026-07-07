using UnityEngine;

public class VNAudioManager : MonoBehaviour
{
    public static VNAudioManager Instance;

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip backgroundTrack;

    [Header("SFX Source")]
    public AudioSource sfxSource;

    [Header("UI SFX")]
    public AudioClip clickClip;
    public AudioClip backgroundChangeClip;
    public AudioClip skipClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (musicSource != null && backgroundTrack != null)
        {
            musicSource.clip = backgroundTrack;
            musicSource.loop = true;
            musicSource.volume = 0.4f;
            musicSource.Play();
        }
    }

    // play helpers
    public void PlayClick()             => Play(clickClip);
    public void PlayBgChange()          => Play(backgroundChangeClip);
    public void PlaySkip()              => Play(skipClip);

    void Play(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }
}