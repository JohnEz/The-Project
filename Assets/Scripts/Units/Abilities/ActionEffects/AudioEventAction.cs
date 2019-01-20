using UnityEngine;

[CreateAssetMenu(fileName = "New Audio Effect", menuName = "Card/EventFX/Audio Effect")]
public class AudioEventAction : EventAction {
    public AudioClip audioClip = null;

    // Use this for initialization
    public AudioEventAction() : base() {
        action = (UnitController caster, Node targetedTile) => {
            Transform targetTransform = caster.transform;
            switch (eventTarget) {
                case EventTarget.TARGETUNIT:
                case EventTarget.TARGETEDTILE:
                    targetTransform = targetedTile.transform;
                    break;
            };
            PlayOptions soundOptions = new PlayOptions(audioClip, targetTransform);
            soundOptions.audioMixer = AudioMixers.SFX;
            AudioManager.singleton.Play(soundOptions);
        };
    }
}