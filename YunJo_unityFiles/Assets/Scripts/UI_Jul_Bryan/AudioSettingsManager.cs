using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private AudioMixer mixer;

    private void Start()
{


    float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
    float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
    float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

    masterSlider.value = master;
    musicSlider.value = music;
    sfxSlider.value = sfx;

    SetMasterVolume(master); //apply saved volume settings
    SetMusicVolume(music);
    SetSFXVolume(sfx);
}

    public void SetMasterVolume(float volume)
    {
        Debug.Log(volume);
        volume = Mathf.Clamp(volume, 0.0001f, 1f); //prevent slider from truly reaching 0 since log(10) is undefined
        mixer.SetFloat("MasterVolume", Mathf.Log10(volume)*20);  //convert into linear scale
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
