using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Increase Speed")]
public class IncreaseSpeed : AttackEffect {
    public int turns = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        TargetUnit.ApplyBuff(new SpeedBuff(turns));
    }
}