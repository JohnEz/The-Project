using UnityEngine;

[CreateAssetMenu(fileName = "New Visual Effect", menuName = "Card/EventFX/Visual Effect")]
public class VisualEffectEventAction : EventAction {
    public GameObject effectObject = null;
    public float delay = 0;

    // Use this for initialization
    public VisualEffectEventAction() : base() {
        action = (UnitController caster, UnitController target, Node targetedTile) => {
            switch (eventTarget) {
                case EventTarget.CASTER:
                    if (caster != null) {
                        caster.CreateEffectWithDelay(effectObject, delay);
                    }
                    break;
                case EventTarget.TARGETUNIT:
                    if (target != null) {
                        Debug.Log("target unit create effect called");
                        caster.CreateEffectWithDelay(effectObject, delay, target.myNode);
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