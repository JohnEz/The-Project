using UnityEngine;

public class DamageMod : ScriptableObject {

    public virtual int Apply(int damage, UnitController caster, UnitController target) {
        return damage;
    }
}