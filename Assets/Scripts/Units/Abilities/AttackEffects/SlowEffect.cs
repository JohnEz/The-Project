using UnityEngine;

[CreateAssetMenu(fileName = "New Slow Action", menuName = "Ability/Attack/Slow")]
public class SlowEffect : AttackEffect {
    public int duration = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        TargetUnit.ApplyBuff(new Slow(duration));
    }
}