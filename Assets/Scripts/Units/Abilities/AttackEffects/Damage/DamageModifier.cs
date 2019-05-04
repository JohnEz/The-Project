using UnityEngine;

public class DamageMod : ScriptableObject {

    public virtual float Apply(float damage, UnitController caster, UnitController target) {
        return damage;
    }
}