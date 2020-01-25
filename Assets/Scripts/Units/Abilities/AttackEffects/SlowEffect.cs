using UnityEngine;

[CreateAssetMenu(fileName = "New Slow Action", menuName = "Ability/Attack/Slow")]
public class SlowEffect : AttackEffect {
    public int duration = 1;

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);
        targetNode.MyUnit.ApplyBuff(new Slow(duration));
    }
}