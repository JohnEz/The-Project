using UnityEngine;
using System.Collections;

public class AttackEffect : ScriptableObject {

    public virtual void AbilityEffect(UnitController caster, UnitController unit) {

    }

    public virtual void AbilityEffect(UnitController caster, Node targetNode) {

    }

    public virtual string ToDescription() {
        return null;
    }

}
