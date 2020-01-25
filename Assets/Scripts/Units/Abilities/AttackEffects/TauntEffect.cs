using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Taunt")]
public class TauntEffect : AttackEffect {
    public int turns = 1;

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);
        targetNode.MyUnit.ApplyBuff(new Taunt(turns, caster));
    }
}