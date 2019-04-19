using UnityEngine;

public class MainMenuController : MonoBehaviour {
    public AudioClip buttonClickAudio;

    public void Start() {
        AudioManager.instance.PlayMusic("Menu", true);
    }

    public void Play() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.instance.Play(pressAudioOptions);

        transform.parent.Find("PlayMenu").GetComponent<PlayMenuController>().OpenMenu();
        CloseMenu(Vector2.left);
    }

    public void Options() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.instance.Play(pressAudioOptions);

        transform.parent.Find("OptionsMenu").GetComponent<OptionsMenuController>().OpenMenu();
        CloseMenu(Vector2.down);
    }

    public void OpenMenu() {
        GetComponent<SlidingMenu>().OpenMenu();
    }

    public void CloseMenu(Vector2 direction) {
        GetComponent<SlidingMenu>().CloseMenu(direction);
    }

    public void Exit() {
        Application.Quit();
    }
}