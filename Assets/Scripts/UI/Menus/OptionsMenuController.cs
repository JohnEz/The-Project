using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour {
    public static Vector3 OPEN_POSITION = new Vector3(0, 0, 0);

    public AudioMixer masterMixer;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    public AudioClip buttonClickAudio;

    private Resolution[] resolutions;

    public const string FULL_SCREEN = "fullScreen";

    private void Start() {
        LoadSettings();
    }

    public void OpenMenu() {
        //GetComponent<SlidingMenu>().SlideToPosition(OPEN_POSITION);
        GetComponent<SlidingMenu>().OpenMenu();
    }

    public void CloseMenu() {
        GetComponent<SlidingMenu>().CloseMenu();
    }

    public void LoadSettings() {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++) {
            Resolution res = resolutions[i];
            string option = string.Format("{0} x {1}", res.width, res.height);

            // TODO this can be removed when i add refresh rate
            options.Add(option);

            if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = PlayerPrefs.GetInt(FULL_SCREEN) == 1;
        Screen.fullScreen = PlayerPrefs.GetInt(FULL_SCREEN) == 1;

        RefreshMusicSliders();
    }

    public void RefreshMusicSliders() {
        float loadedMasterVolume = PlayerPrefs.GetFloat(AudioManager.MASTER_VOLUME);
        masterVolumeSlider.value = loadedMasterVolume;

        float loadedMusicVolume = PlayerPrefs.GetFloat(AudioManager.MUSIC_VOLUME);
        musicVolumeSlider.value = loadedMusicVolume;

        float loadedSFXVolume = PlayerPrefs.GetFloat(AudioManager.SFX_VOLUME);
        sfxVolumeSlider.value = loadedSFXVolume;
    }

    public void SetMasterVolume(float volume) {
        masterMixer.SetFloat(AudioManager.MASTER_VOLUME, volume);
    }

    public void SetMusicVolume(float volume) {
        masterMixer.SetFloat(AudioManager.MUSIC_VOLUME, volume);
    }

    public void SetSfxVolume(float volume) {
        masterMixer.SetFloat(AudioManager.SFX_VOLUME, volume);
    }

    public void SetFullScreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int index) {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // TODO add option for not capped
        //Application.targetFrameRate = resolution.refreshRate;
    }

    public void SaveAndExit() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.singleton.Play(pressAudioOptions);

        SaveOptions();

        transform.parent.Find("MainMenu").GetComponent<MainMenuController>().OpenMenu();
        CloseMenu();
    }

    public void SaveOptions() {
        float masterVolume = 0;
        masterMixer.GetFloat(AudioManager.MASTER_VOLUME, out masterVolume);
        PlayerPrefs.SetFloat(AudioManager.MASTER_VOLUME, masterVolume);

        float musicVolume = 0;
        masterMixer.GetFloat(AudioManager.MUSIC_VOLUME, out musicVolume);
        PlayerPrefs.SetFloat(AudioManager.MUSIC_VOLUME, musicVolume);

        float sfxVolume = 0;
        masterMixer.GetFloat(AudioManager.SFX_VOLUME, out sfxVolume);
        PlayerPrefs.SetFloat(AudioManager.SFX_VOLUME, sfxVolume);

        PlayerPrefs.SetInt(FULL_SCREEN, fullscreenToggle.isOn ? 1 : 0);
    }
}