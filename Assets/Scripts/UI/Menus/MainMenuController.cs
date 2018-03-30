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
		MenuSystem.LoadScene (Scenes.ARENA);
	}

	public void ExitGame () {
		Application.Quit ();
	}
}

