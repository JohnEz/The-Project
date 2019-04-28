using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Heal")]
public class HealEffect : AttackEffect {
    public int healing = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        caster.GiveHealingTo(TargetUnit, healing);
    }
}