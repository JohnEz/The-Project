using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Audio Effect", menuName = "Card/EventFX/Audio Effect")]
public class AudioEventAction : EventAction {

    public AudioClip audioClip = null;

    // Use this for initialization
    public AudioEventAction() : base() {
        action = (UnitController caster, UnitController target, Node targetedTile) => {
            switch (eventTarget) {
                case EventTarget.CASTER:
                    caster.PlayOneShot(audioClip);
                    break;
                case EventTarget.TARGETUNIT:
                    target.PlayOneShot(audioClip);
                    break;
                case EventTarget.TARGETEDTILE:
                    //TODO ADD SOUNDEFFECT TO NODE
                    break;
            };
        };
    }
}
