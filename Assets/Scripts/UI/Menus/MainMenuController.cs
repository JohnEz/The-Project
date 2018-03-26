using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

	public void PlayGameAI () {
		MatchDetails.VersusAi = true;
		LoadArena ();
	}

	public void PlayGameVersus () {
		MatchDetails.VersusAi = false;
		LoadArena ();
	}

	void LoadArena() {
		MenuSystem.SceneToLoad = Scenes.ARENA;
		SceneManager.LoadScene (Scenes.LOADING);
	}

	public void ExitGame () {
		Application.Quit ();
	}
}

