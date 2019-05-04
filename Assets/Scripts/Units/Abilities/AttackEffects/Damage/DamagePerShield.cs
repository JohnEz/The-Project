using UnityEngine;

[CreateAssetMenu(fileName = "Per Shield Mod", menuName = "Ability/Attack/Damage/Per Shield")]
public class DamagePerShield : DamageMod {

    public override float Apply(float damage, UnitController caster, UnitController target) {
        return damage * caster.Shield;
    }
}