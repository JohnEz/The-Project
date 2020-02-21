using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Exposed Weakness")]
public class ExposedWeaknessEffect : AttackEffect {

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);
        targetNode.MyUnit.ApplyBuff(new ExposedWeakness(1));
    }
}