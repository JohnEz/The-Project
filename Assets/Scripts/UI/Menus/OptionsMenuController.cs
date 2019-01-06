using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private void Start() {
        LoadSettings();
    }

    public void OpenMenu() {
        GetComponent<SlidingMenu>().SlideToPosition(OPEN_POSITION);
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

        fullscreenToggle.isOn = Screen.fullScreen;
        float startingMasterVolume = 0;
        masterMixer.GetFloat("masterVolume", out startingMasterVolume);
        masterVolumeSlider.value = startingMasterVolume;

        float startingMusicVolume = 0;
        masterMixer.GetFloat("musicVolume", out startingMusicVolume);
        musicVolumeSlider.value = startingMusicVolume;

        float startingSfxVolume = 0;
        masterMixer.GetFloat("sfxVolume", out startingSfxVolume);
        sfxVolumeSlider.value = startingSfxVolume;
    }

    public void SetMasterVolume(float volume) {
        masterMixer.SetFloat("masterVolume", volume);
    }

    public void SetMusicVolume(float volume) {
        masterMixer.SetFloat("musicVolume", volume);
    }

    public void SetSfxVolume(float volume) {
        masterMixer.SetFloat("sfxVolume", volume);
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
        AudioManager.singleton.Play(buttonClickAudio, transform, AudioMixers.UI, true);

        transform.parent.Find("MainMenu").GetComponent<MainMenuController>().OpenMenu();
        CloseMenu();
    }
}
