using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Shield")]
public class ShieldEffect : AttackEffect {
    public int shield = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        caster.GiveShieldTo(TargetUnit, shield);
    }
}