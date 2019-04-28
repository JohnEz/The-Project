using UnityEngine;

[CreateAssetMenu(fileName = "New Visual Effect", menuName = "Card/EventFX/Visual Effect")]
public class VisualEffectEventAction : EventAction {
    public GameObject effectObject = null;
    public float delay = 0;

    public bool rotateWithCharacter = false;

    // Use this for initialization
    public VisualEffectEventAction() : base() {
        action = (UnitController caster, Node targetedTile) => {
            EffectOptions options = new EffectOptions(effectObject, delay);
            options.rotateWithCharacter = rotateWithCharacter;
            switch (eventTarget) {
                case EventTarget.CASTER:
                    if (caster != null) {
                        caster.CreateEffect(options);
                    }
                    break;

                case EventTarget.TARGETUNIT:
                    if (targetedTile != null) {
                        options.location = targetedTile;
                        caster.CreateEffect(options);
                    }
                    break;

                case EventTarget.TARGETEDTILE:
                    if (caster != null) {
                        options.location = targetedTile;
                        caster.CreateEffect(options);
                    }
                    break;
            };
        };
    }
}