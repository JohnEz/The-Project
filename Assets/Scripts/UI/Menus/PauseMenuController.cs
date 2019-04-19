using UnityEngine;

public class PauseMenuController : MonoBehaviour {
    public static PauseMenuController instance;

    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;

    public AudioClip buttonClickAudio;

    public void Awake() {
        instance = this;
    }

    public void Pause() {
        Time.timeScale = 0f;
        gameIsPaused = true;
        pauseMenuUI.SetActive(true);
    }

    public void Resume() {
        Time.timeScale = 1f;
        gameIsPaused = false;
        pauseMenuUI.SetActive(false);
    }

    public void LoadMainMenu() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.instance.Play(pressAudioOptions);

        Resume();
        SceneChanger.Instance.FadeToScene(Scenes.MAIN_MENU);
    }

    public void ExitGame() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.instance.Play(pressAudioOptions);

        Application.Quit();
    }
}