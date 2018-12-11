using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Visual Effect", menuName = "Card/EventFX/Visual Effect")]
public class VisualEffectEventAction : EventAction {

    public GameObject effectObject = null;
    public float delay = 0;

    // Use this for initialization
    public VisualEffectEventAction() : base() {
        action = (UnitController caster, UnitController target, Node targetedTile) => {
            switch (eventTarget) {
                case EventTarget.CASTER:
                    caster.CreateEffectWithDelay(effectObject, delay);
                    break;
                case EventTarget.TARGETUNIT:
                    target.CreateEffectWithDelay(effectObject, delay);
                    break;
                case EventTarget.TARGETEDTILE:
                    caster.CreateEffectWithDelay(effectObject, delay, targetedTile);
                    break;
            };
        };
    }
}
