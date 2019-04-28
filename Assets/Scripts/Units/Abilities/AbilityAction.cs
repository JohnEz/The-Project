using System.Collections.Generic;
using UnityEngine;

// type of units the ability effects
public enum TargetType {
    ENEMY,
    ALLY,
    UNIT //both enemies and allies
}

// type of object that must be the center of the attack
public enum TileTarget {
    TILE,
    UNIT,
    EMPTY_TILE
}

// type of area of effect
public enum AreaOfEffect {
    SINGLE,
    CIRCLE,
    CLEAVE,
    AURA
}

public enum AbilityEvent {
    CAST_START,
    CAST_END, //called just for the caster after cast
    HIT //called for each target on hit TODO, doesnt seem to be called anywhere
}

public enum EventTarget {
    TARGETEDTILE,
    CASTER,
    TARGETUNIT
}

public struct EventActionLegacy {
    public AbilityEvent eventTrigger;
    public EventTarget eventTarget;
    public System.Action<UnitController, UnitController, Node> action;

    public static EventAction CreateProjectileEventAction(AbilityEvent _eventTrigger, GameObject projectileObject, float speed, float delay = 0) {
        EventAction newEventAction = new EventAction();
        newEventAction.eventTrigger = _eventTrigger;
        newEventAction.eventTarget = EventTarget.CASTER;
        newEventAction.action = (UnitController caster, Node targetedTile) => {
            caster.CreateProjectileWithDelay(projectileObject, targetedTile, speed, delay);
        };

        return newEventAction;
    }
}

[System.Serializable]
public class AbilityAction : ScriptableObject {

    [SerializeField]
    public List<EventAction> eventActions = new List<EventAction>();

    [HideInInspector]
    public UnitController caster;

    public string description = null;

    public void AddAbilityTarget(Node targetNode, System.Action ability) {
        caster.AddAbilityTarget(targetNode, ability);
    }
}