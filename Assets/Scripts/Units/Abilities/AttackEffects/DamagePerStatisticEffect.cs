using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Damage per Statistic")]
public class DamagePerStatisticEffect : AttackEffect {
    public int baseDamage = 1;

    public StatisticTypes modifier = StatisticTypes.NONE;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        int damage = baseDamage * caster.myCounters.GetStatistic(modifier);
        caster.DealDamageTo(TargetUnit, damage);
    }
}