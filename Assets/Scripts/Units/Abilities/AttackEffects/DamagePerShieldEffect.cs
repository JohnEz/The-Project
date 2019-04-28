using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Damage Per Shield")]
public class DamagePerShieldEffect : AttackEffect {
    public int damageMod = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        caster.DealDamageTo(TargetUnit, caster.Shield * damageMod);
    }
}