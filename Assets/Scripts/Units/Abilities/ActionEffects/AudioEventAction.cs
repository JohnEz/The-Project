using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Audio Effect", menuName = "Card/EventFX/Audio Effect")]
public class AudioEventAction : EventAction {

    public AudioClip audioClip = null;

    // Use this for initialization
    public AudioEventAction() : base() {
        action = (UnitController caster, UnitController target, Node targetedTile) => {
            Transform targetTransform = caster.transform;
            switch (eventTarget) {
                case EventTarget.TARGETUNIT:
                    if (target != null) {
                        targetTransform = target.transform;
                    }
                    break;
                case EventTarget.TARGETEDTILE:
                    targetTransform = targetedTile.transform;
                    break;
            };

            AudioManager.singleton.Play(audioClip, targetTransform, AudioMixers.SFX);
        };
    }
}
