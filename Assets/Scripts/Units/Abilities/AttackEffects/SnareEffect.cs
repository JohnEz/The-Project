using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Snare")]
public class SnareEffect : AttackEffect {
    public int duration = 1;

    public GameObject effectPrefab;

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);
        targetNode.MyUnit.ApplyBuff(new Snare(duration, effectPrefab));
    }
}