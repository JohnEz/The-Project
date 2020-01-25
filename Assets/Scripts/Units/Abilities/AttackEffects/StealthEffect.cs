using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Stealth")]
public class StealthEffect : AttackEffect {
    public int turns = 1;

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);
        targetNode.MyUnit.ApplyBuff(new Stealth(turns));
    }
}