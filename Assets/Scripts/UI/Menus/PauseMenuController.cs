using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
	public static bool gameIsPaused = false;

	public GameObject pauseMenuUI;

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
		Resume ();
		MenuSystem.LoadScene (Scenes.MAIN_MENU);
	}

	public void ExitGame () {
		Application.Quit ();
	}
}

