using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    Vector3 EXIT_POSITION = new Vector3(-1280, 0, 0);

    public AudioClip buttonClickAudio;

    public void Start() {
        AudioManager.singleton.PlayMusic("Menu", true);
    }

    public void Play() {
        AudioManager.singleton.Play(buttonClickAudio, transform, AudioMixers.UI, true);

        transform.parent.Find("PlayMenu").GetComponent<PlayMenuController>().OpenMenu();
        CloseMenu();
    }

    public void Options() {
        AudioManager.singleton.Play(buttonClickAudio, transform, AudioMixers.UI, true);

        transform.parent.Find("OptionsMenu").GetComponent<OptionsMenuController>().OpenMenu();
        CloseMenu();
    }

    public void OpenMenu() {
        GetComponent<SlidingMenu>().CloseMenu();
    }

    public void CloseMenu() {
        GetComponent<SlidingMenu>().SlideToPosition(EXIT_POSITION);
    }

	public void Exit() {
		Application.Quit ();
	}
}

