using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Heal/Healing")]
public class HealEffect : AttackEffect {
    public int HealMin = 1;
    public int HealMax = 1;

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);

        int healing = Random.Range(HealMin, HealMax) + caster.myStats.Wisdom;

        caster.GiveHealingTo(targetNode.MyUnit, healing);
    }
}