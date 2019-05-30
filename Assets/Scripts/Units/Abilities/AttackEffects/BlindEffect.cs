using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Blind")]
public class BlindEffect : AttackEffect {

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        TargetUnit.ApplyBuff(new Blind());
    }
}