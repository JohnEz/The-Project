using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour {
    public AudioClip buttonClickAudio;

    //Temp
    public List<UnitObject> defaultCharacters;

    public void Start() {
        AudioManager.instance.PlayMusic("Menu", true);
    }

    public void NewGame() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.instance.Play(pressAudioOptions);

        // TEMP add starting characters to a string list in static data
        if (PlayerSchool.Roster.Count <= 0) {
            defaultCharacters.ForEach((character) => {
                PlayerSchool.Roster.Add(Instantiate(character));
            });
        }

        SceneChanger.Instance.FadeToScene(Scenes.PRE_GAME);
    }

    public void LoadGame() {
    }

    public void Options() {
        PlayOptions pressAudioOptions = new PlayOptions(buttonClickAudio, transform);
        pressAudioOptions.audioMixer = AudioMixers.UI;
        pressAudioOptions.persist = true;
        AudioManager.instance.Play(pressAudioOptions);
    }

    public void Exit() {
        Application.Quit();
    }
}