using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour {
    public static PauseMenuController singleton;

	public static bool gameIsPaused = false;

	public GameObject pauseMenuUI;

    public AudioClip buttonClickAudio;

    public void Awake() {
        singleton = this;
    }

    public void Pause () {
		Time.timeScale = 0f;
		gameIsPaused = true;
		pauseMenuUI.SetActive (true);
	}

	public void Resume () {
		Time.timeScale = 1f;
		gameIsPaused = false;	
		pauseMenuUI.SetActive (false);
	}

	public void LoadMainMenu () {
        AudioManager.singleton.Play(buttonClickAudio, transform, AudioMixers.UI, true);

        Resume ();
		MenuSystem.LoadScene (Scenes.MAIN_MENU);
	}

	public void ExitGame () {
        AudioManager.singleton.Play(buttonClickAudio, transform, AudioMixers.UI, true);

        Application.Quit ();
	}
}

