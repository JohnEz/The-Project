using UnityEngine;

[CreateAssetMenu(fileName = "Per Statistic Mod", menuName = "Ability/Attack/Damage/Per Statistic")]
public class DamagePerStatistic : DamageMod {
    public StatisticTypes modifier = StatisticTypes.NONE;

    public override float Apply(float damage, UnitController caster, UnitController target) {
        return damage * caster.myCounters.GetStatistic(modifier);
    }
}