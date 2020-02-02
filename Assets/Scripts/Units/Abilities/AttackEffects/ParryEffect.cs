using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Parry")]
public class ParryEffect : AttackEffect {
    public AttackAction parryAttack;

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);
        AttackAction attack = Instantiate(parryAttack);
        attack.caster = caster;
        targetNode.MyUnit.ApplyBuff(new Parry(1, attack));
    }
}