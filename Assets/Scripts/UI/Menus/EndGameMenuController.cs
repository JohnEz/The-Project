using UnityEngine;

public class EndGameMenuController : MonoBehaviour {
    public AudioClip buttonClickAudio;

    public void Quit() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.instance.Play(pressAudioOptions);

        SceneChanger.Instance.FadeToScene(Scenes.MAIN_MENU);
    }

    public void Retry() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.instance.Play(pressAudioOptions);

        SceneChanger.Instance.FadeToScene(Scenes.BATTLE);
    }
}