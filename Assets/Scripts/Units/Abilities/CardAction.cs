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
    HIT //called for each target on hit TODO, doesnt seem to be called anywhere
}

public enum EventTarget
{
    TARGETEDTILE,
    CASTER,
    TARGETUNIT
}

public struct EventActionLegacy
{
    public AbilityEvent eventTrigger;
    public EventTarget eventTarget;
    public System.Action<UnitController, UnitController, Node> action;

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

    [SerializeField]
    public List<EventAction> eventActions = new List<EventAction>();

    public UnitController caster;

    public void AddAbilityTarget(UnitController target, System.Action ability) {
        caster.AddAbilityTarget(target, ability);
    }

}
