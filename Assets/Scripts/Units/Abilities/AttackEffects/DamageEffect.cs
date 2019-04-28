using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Damage")]
public class DamageEffect : AttackEffect {
    public int damage = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        caster.DealDamageTo(TargetUnit, damage);
    }
}