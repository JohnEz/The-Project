using UnityEngine;

[CreateAssetMenu(fileName = "New Visual Effect", menuName = "Card/EventFX/Visual Effect")]
public class VisualEffectEventAction : EventAction {
    public GameObject effectObject = null;
    public float delay = 0;

    // Use this for initialization
    public VisualEffectEventAction() : base() {
        action = (UnitController caster, Node targetedTile) => {
            switch (eventTarget) {
                case EventTarget.CASTER:
                    if (caster != null) {
                        caster.CreateEffectWithDelay(effectObject, delay);
                    }
                    break;

                case EventTarget.TARGETUNIT:
                    if (targetedTile != null) {
                        caster.CreateEffectWithDelay(effectObject, delay, targetedTile);
                    }
                    break;

                case EventTarget.TARGETEDTILE:
                    if (caster != null) {
                        caster.CreateEffectWithDelay(effectObject, delay, targetedTile);
                    }
                    break;
            };
        };
    }
}