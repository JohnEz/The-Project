using UnityEngine;
using System.Collections;

public class EndGameMenuController : MonoBehaviour {

    public AudioClip buttonClickAudio;

    public void Quit() {
        AudioManager.singleton.Play(buttonClickAudio, transform, AudioMixers.UI, true);

        SceneChanger.Instance.FadeToScene(Scenes.MAIN_MENU);
    }

    public void Retry() {
        AudioManager.singleton.Play(buttonClickAudio, transform, AudioMixers.UI, true);

        SceneChanger.Instance.FadeToScene(Scenes.ARENA);
    }
}
