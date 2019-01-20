using UnityEngine;

public class MainMenuController : MonoBehaviour {
    private Vector3 EXIT_POSITION = new Vector3(-1280, 0, 0);

    public AudioClip buttonClickAudio;

    public void Start() {
        AudioManager.singleton.PlayMusic("Menu", true);
    }

    public void Play() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.singleton.Play(pressAudioOptions);

        transform.parent.Find("PlayMenu").GetComponent<PlayMenuController>().OpenMenu();
        CloseMenu();
    }

    public void Options() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.singleton.Play(pressAudioOptions);

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
        Application.Quit();
    }
}