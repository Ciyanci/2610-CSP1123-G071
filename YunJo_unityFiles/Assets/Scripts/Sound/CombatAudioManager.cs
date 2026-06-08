using UnityEngine;
public class CombatAudioManager : MonoBehaviour
{
    public static CombatAudioManager Instance;
    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip   backgroundTrack;
    [Header("SFX Source")]
    public AudioSource sfxSource;
    [Header("Combat SFX")]
    public AudioClip clashHitClip;
    public AudioClip diceRollClip;
    public AudioClip speedDiceRollClip;
    public AudioClip turnBeginClip;
    public AudioClip unopposedHitClip;

    [Header("UI SFX")]
    public AudioClip cardHoverClip;
    public AudioClip targetSelectClip;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (musicSource != null && backgroundTrack != null)
        {
            musicSource.clip   = backgroundTrack;
            musicSource.loop   = true;
            musicSource.volume = 0.4f;
            musicSource.Play();
        }
    }
    //play helper**
    public void PlayClashHit()      => Play(clashHitClip);
    public void PlayDiceRoll()      => Play(diceRollClip);
    public void PlaySpeedDiceRoll() => Play(speedDiceRollClip);
    public void PlayTurnBegin()     => Play(turnBeginClip);
    public void PlayCardHover()     => Play(cardHoverClip);
    public void PlayTargetSelect()  => Play(targetSelectClip);
    public void PlayUnopposedHit() => Play(unopposedHitClip);
    void Play(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }
}