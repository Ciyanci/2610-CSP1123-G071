using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    private void Start()
{


    float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
    float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
    float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

    SetMasterVolume(master);
    SetMusicVolume(music);
    SetSFXVolume(sfx);
}

    public void SetMasterVolume(float volume)
    {
        Debug.Log(volume);
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        mixer.SetFloat("MasterVolume", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        Debug.Log(volume);
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        Debug.Log(volume);
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        mixer.SetFloat("SFXVolume", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

}
