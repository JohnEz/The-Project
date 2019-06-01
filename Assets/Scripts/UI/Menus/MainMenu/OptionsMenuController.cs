using DuloGames.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour {
    public static Vector3 OPEN_POSITION = new Vector3(0, 0, 0);

    public AudioMixer masterMixer;
    public UISelectField resolutionSelectField;
    public Toggle fullscreenToggle;
    public Toggle mouseCanControlCameraToggle;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    public AudioClip buttonClickAudio;

    private Resolution[] resolutions;

    public const string FULL_SCREEN = "fullScreen";
    public const string MOUSE_CAN_CONTROL_CAMERA = "mouseCanControlCamera";

    private void Start() {
        LoadSettings();
    }

    public void LoadSettings() {
        SetupResolution();

        int fullscreenValue = PlayerPrefs.GetInt(FULL_SCREEN);
        if (fullscreenValue == 0) {
            fullscreenToggle.isOn = Screen.fullScreen;
        } else {
            bool isFullscreen = fullscreenValue == 1;
            fullscreenToggle.isOn = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }

        int mouseCanControlCameraValue = PlayerPrefs.GetInt(MOUSE_CAN_CONTROL_CAMERA);
        if (fullscreenValue == 0) {
            mouseCanControlCameraToggle.isOn = true;
            GameSettings.MouseCanMoveCamera = true;
        } else {
            bool mouseCanControlCamera = mouseCanControlCameraValue == 1;
            mouseCanControlCameraToggle.isOn = mouseCanControlCamera;
            GameSettings.MouseCanMoveCamera = mouseCanControlCamera;
        }

        RefreshMusicSliders();
    }

    protected void OnEnable() {
        if (resolutionSelectField == null)
            return;

        resolutionSelectField.onChange.AddListener(OnResolutionSelectedOption);
    }

    protected void OnDisable() {
        if (resolutionSelectField == null)
            return;

        resolutionSelectField.onChange.RemoveListener(OnResolutionSelectedOption);
    }

    protected void OnResolutionSelectedOption(int index, string option) {
        Resolution res = Screen.resolutions[index];

        if (res.Equals(Screen.currentResolution))
            return;

        Screen.SetResolution(res.width, res.height, true, res.refreshRate);
    }

    public void SetupResolution() {
        if (resolutionSelectField == null) {
            return;
        }

        resolutionSelectField.ClearOptions();

        resolutions = Screen.resolutions;

        foreach (Resolution res in resolutions) {
            resolutionSelectField.AddOption(string.Format("{0} x {1} @{2}Hz", res.width, res.height, res.refreshRate));
        }

        Resolution currentResolution = Screen.currentResolution;

        resolutionSelectField.SelectOption(string.Format("{0} x {1} @{2}Hz", currentResolution.width, currentResolution.height, currentResolution.refreshRate));
    }

    public void RefreshMusicSliders() {
        float loadedMasterVolume = PlayerPrefs.GetFloat(AudioManager.MASTER_VOLUME);
        masterVolumeSlider.value = (loadedMasterVolume / 80) + 1;

        float loadedMusicVolume = PlayerPrefs.GetFloat(AudioManager.MUSIC_VOLUME);
        musicVolumeSlider.value = (loadedMusicVolume / 80) + 1;

        float loadedSFXVolume = PlayerPrefs.GetFloat(AudioManager.SFX_VOLUME);
        sfxVolumeSlider.value = (loadedSFXVolume / 80) + 1;
    }

    public void SetMasterVolume(float volume) {
        masterMixer.SetFloat(AudioManager.MASTER_VOLUME, (volume - 1) * 80);
    }

    public void SetMusicVolume(float volume) {
        masterMixer.SetFloat(AudioManager.MUSIC_VOLUME, (volume - 1) * 80);
    }

    public void SetSfxVolume(float volume) {
        masterMixer.SetFloat(AudioManager.SFX_VOLUME, (volume - 1) * 80);
    }

    public void SetFullScreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }

    public void SetMouseCameraControl(bool mouseCanControlCamera) {
        GameSettings.MouseCanMoveCamera = mouseCanControlCamera;
    }

    public void SaveAndExit() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.instance.Play(pressAudioOptions);

        SaveOptions();
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

        PlayerPrefs.SetInt(FULL_SCREEN, fullscreenToggle.isOn ? 1 : -1);

        PlayerPrefs.SetInt(MOUSE_CAN_CONTROL_CAMERA, mouseCanControlCameraToggle.isOn ? 1 : -1);
    }
}