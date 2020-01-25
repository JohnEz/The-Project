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

    public virtual int GetDamage(UnitController caster) {
        return 0;
    }

    public virtual int GetHealing(UnitController caster) {
        return 0;
    }

    public virtual int GetShield(UnitController caster) {
        return 0;
    }
}