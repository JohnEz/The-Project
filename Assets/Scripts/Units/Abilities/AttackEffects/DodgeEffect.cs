using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Dodge")]
public class DodgeEffect : AttackEffect {

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);
        targetNode.MyUnit.ApplyBuff(new Dodge(1));
    }
}