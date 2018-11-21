using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetType
{
    ENEMY,
    ALLY,
    UNIT //both enemies and allies
}

public enum TileTarget
{
    TILE,
    UNIT
}

public enum AreaOfEffect
{
    SINGLE,
    CIRCLE,
    CLEAVE,
    AURA
}

public enum AbilityEvent
{
    CAST_START,
    CAST_END, //called just for the caster after cast
    HIT //called for each target on hit
}

public enum EventTarget
{
    TARGETEDTILE,
    CASTER,
    TARGETUNIT
}

public struct EventAction
{
    public AbilityEvent eventTrigger;
    public EventTarget eventTarget;
    public System.Action<UnitController, UnitController, Node> action;

    public static EventAction CreateAudioEventAction(AbilityEvent _eventTrigger, AudioClip audioClip, EventTarget _eventTarget)
    {
        EventAction newEventAction = new EventAction();
        newEventAction.eventTrigger = _eventTrigger;
        newEventAction.eventTarget = _eventTarget;
        newEventAction.action = (UnitController caster, UnitController target, Node targetedTile) => {
            switch (_eventTarget)
            {
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

        return newEventAction;
    }

    public static EventAction CreateEffectEventAction(AbilityEvent _eventTrigger, GameObject effectObject, EventTarget _eventTarget, float delay = 0)
    {
        EventAction newEventAction = new EventAction();
        newEventAction.eventTrigger = _eventTrigger;
        newEventAction.eventTarget = _eventTarget;
        newEventAction.action = (UnitController caster, UnitController target, Node targetedTile) => {
            switch (_eventTarget)
            {
                case EventTarget.CASTER:
                    caster.CreateEffectWithDelay(effectObject, delay);
                    break;
                case EventTarget.TARGETUNIT:
                    target.CreateEffectWithDelay(effectObject, delay);
                    break;
            };
        };

        return newEventAction;
    }

    public static EventAction CreateEffectAtLocationEventAction(AbilityEvent _eventTrigger, GameObject effectObject, float delay = 0)
    {
        EventAction newEventAction = new EventAction();
        newEventAction.eventTrigger = _eventTrigger;
        newEventAction.eventTarget = EventTarget.TARGETEDTILE;
        newEventAction.action = (UnitController caster, UnitController target, Node targetedTile) => {
            caster.CreateEffectWithDelay(effectObject, delay, targetedTile);
        };

        return newEventAction;
    }

    public static EventAction CreateProjectileEventAction(AbilityEvent _eventTrigger, GameObject projectileObject, float speed, float delay = 0)
    {
        EventAction newEventAction = new EventAction();
        newEventAction.eventTrigger = _eventTrigger;
        newEventAction.eventTarget = EventTarget.CASTER;
        newEventAction.action = (UnitController caster, UnitController target, Node targetedTile) => {
            caster.CreateProjectileWithDelay(projectileObject, target.myNode, speed, delay);
        };

        return newEventAction;
    }
}

[System.Serializable]
public class CardAction : ScriptableObject {

    public List<EventAction> eventActions = new List<EventAction>();

    public UnitController caster;

    //public CardAction(List<EventAction> _eventActions, UnitController _caster) {
    //    eventActions = _eventActions;
    //    caster = _caster;
    //}

    //public virtual void UseAbility(Node target) {
    //    eventActions.ForEach((eventAction) => {
    //        if (eventAction.eventTrigger == AbilityEvent.CAST_START)
    //        {
    //            eventAction.action(caster, target.myUnit, target);
    //        }
    //    });
    //}

    //public virtual void UseAbility(List<Node> targets, Node target) {
    //    // TODO this may need to be moved to attack version
    //    eventActions.ForEach((eventAction) => {
    //        if (eventAction.eventTrigger == AbilityEvent.CAST_START)
    //        {
    //            if (eventAction.eventTarget == EventTarget.CASTER || eventAction.eventTarget == EventTarget.TARGETEDTILE)
    //            {
    //                eventAction.action(caster, null, target);
    //            }
    //            else if (eventAction.eventTarget == EventTarget.TARGETUNIT)
    //            {
    //                targets.ForEach((targetNode) => {
    //                    //if (CanHitUnit(targetNode)) {
    //                        eventAction.action(caster, targetNode.myUnit, target);
    //                    //}
    //                });
    //            }
    //        }
    //    });
    //}

    public void AddAbilityTarget(UnitController target, System.Action ability) {
        caster.AddAbilityTarget(target, ability);
    }

}
