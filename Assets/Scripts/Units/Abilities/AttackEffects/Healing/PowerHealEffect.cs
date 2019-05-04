using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Power Heal")]
public class PowerHealEffect : AttackEffect {
    public float powerMod = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        int healing = PowerModToInt(caster.myStats.Power, powerMod);
        caster.GiveHealingTo(TargetUnit, healing);
    }
}