using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Shield")]
public class ShieldEffect : AttackEffect {
    public float powerMod = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        int shielding = PowerModToInt(caster.myStats.Power, powerMod);
        caster.GiveShieldTo(TargetUnit, shielding);
    }
}