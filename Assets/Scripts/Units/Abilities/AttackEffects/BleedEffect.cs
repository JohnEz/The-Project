using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Bleed")]
public class BleedEffect : AttackEffect {

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        TargetUnit.ApplyBuff(new Bleed());
    }
}