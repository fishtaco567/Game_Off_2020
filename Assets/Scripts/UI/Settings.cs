using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    public float sfxVolume;
    public float musicVolume;

    [SerializeField]
    AudioMixer masterMixer;

    [SerializeField]
    Slider sfxSlider;

    [SerializeField]
    Slider musicSlider;

    // Use this for initialization
    void Start() {
        if(PlayerPrefs.HasKey("sfxvolume")) {
            sfxVolume = PlayerPrefs.GetFloat("sfxvolume");
        } else {
            sfxVolume = 1;
            PlayerPrefs.SetFloat("sfxvolume", sfxVolume);
        }
        sfxSlider.value = sfxVolume;

        if(PlayerPrefs.HasKey("musicvolume")) {
            musicVolume = PlayerPrefs.GetFloat("musicvolume");
        } else {
            musicVolume = 1;
            PlayerPrefs.SetFloat("musicvolume", musicVolume);
        }
        musicSlider.value = musicVolume;
    }

    private void SetVolumes(float sfx, float music) {
        float adjSfx = (1 - sfx) * -80;
        float adjMusic = (1 - music) * -80;
        masterMixer.SetFloat("sfxVol", adjSfx);
        masterMixer.SetFloat("musicVol", adjMusic);
    }

    // Update is called once per frame
    void Update() {

    }

    public void SFXVolumeChange(float volume) {
        sfxVolume = sfxSlider.value;
        PlayerPrefs.SetFloat("sfxvolume", sfxVolume);
        SetVolumes(sfxVolume, musicVolume);
    }

    public void MusicVolumeChange(float volume) {
        musicVolume = musicSlider.value;
        PlayerPrefs.SetFloat("musicvolume", musicVolume);
        SetVolumes(sfxVolume, musicVolume);
    }

    public void OnDestroy() {
        PlayerPrefs.Save();
    }

}
