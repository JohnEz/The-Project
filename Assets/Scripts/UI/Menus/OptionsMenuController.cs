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

    public const string MASTER_VOLUME = "masterVolume";
    public const string MUSIC_VOLUME = "musicVolume";
    public const string SFX_VOLUME = "sfxVolume";
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

        // TODO move this to the audio manager
        float loadedMasterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME);
        masterMixer.SetFloat(MASTER_VOLUME, loadedMasterVolume);
        masterVolumeSlider.value = loadedMasterVolume;

        float loadedMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME);
        masterMixer.SetFloat(MUSIC_VOLUME, loadedMusicVolume);
        musicVolumeSlider.value = loadedMusicVolume;

        float loadedSFXVolume = PlayerPrefs.GetFloat(SFX_VOLUME);
        masterMixer.SetFloat(SFX_VOLUME, loadedSFXVolume);
        sfxVolumeSlider.value = loadedSFXVolume;
    }

    public void SetMasterVolume(float volume) {
        masterMixer.SetFloat(MASTER_VOLUME, volume);
    }

    public void SetMusicVolume(float volume) {
        masterMixer.SetFloat(MUSIC_VOLUME, volume);
    }

    public void SetSfxVolume(float volume) {
        masterMixer.SetFloat(SFX_VOLUME, volume);
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
        masterMixer.GetFloat(MASTER_VOLUME, out masterVolume);
        PlayerPrefs.SetFloat(MASTER_VOLUME, masterVolume);

        float musicVolume = 0;
        masterMixer.GetFloat(MUSIC_VOLUME, out musicVolume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME, musicVolume);

        float sfxVolume = 0;
        masterMixer.GetFloat(SFX_VOLUME, out sfxVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME, sfxVolume);

        PlayerPrefs.SetInt(FULL_SCREEN, fullscreenToggle.isOn ? 1 : 0);
    }
}