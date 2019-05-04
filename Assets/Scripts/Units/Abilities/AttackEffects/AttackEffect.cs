using UnityEngine;

public class AttackEffect : ScriptableObject {
    private UnitController targetUnit;

    public bool targetSelf = false;

    public UnitController TargetUnit {
        get { return targetUnit; }
    }

    public virtual void AbilityEffect(UnitController caster, UnitController unit) {
        targetUnit = targetSelf ? caster : unit;
    }

    public virtual void AbilityEffect(UnitController caster, Node targetNode) {
    }

    public virtual string ToDescription() {
        return null;
    }

    public int PowerModToInt(int power, float mod) {
        return Mathf.RoundToInt(power * mod);
    }
}