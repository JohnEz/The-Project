﻿using DuloGames.UI;
using UnityEngine;

public class PauseMenuController : MonoBehaviour {
    public static PauseMenuController instance;

    public static bool gameIsPaused = false;

    public AudioClip buttonClickAudio;

    public void Awake() {
        instance = this;
    }

    public void Pause() {
        Time.timeScale = 0f;
        gameIsPaused = true;
        UIWindow.GetWindow(UIWindowID.GameMenu).Show();
    }

    public void Resume() {
        PlayButtonSound();

        Time.timeScale = 1f;
        gameIsPaused = false;
        UIWindow.GetWindow(UIWindowID.GameMenu).Hide();
    }

    public void AbandonScenario() {
        PlayButtonSound();

        Resume();
    }

    public void LoadMainMenu() {
        PlayButtonSound();

        Resume();
    }

    public void ExitGame() {
        PlayButtonSound();

        Application.Quit();
    }

    public void PlayButtonSound() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.instance.Play(pressAudioOptions);
    }
}