using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Life Steal")]
public class LifeStealEffect : AttackEffect {
    public int turns = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        TargetUnit.ApplyBuff(new LifeStealBuff(turns));
    }
}