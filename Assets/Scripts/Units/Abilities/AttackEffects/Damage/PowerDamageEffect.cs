using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Damage/Power Damage")]
public class PowerDamageEffect : AttackEffect {
    public float powerMod = 1;

    public List<DamageMod> damageMods = new List<DamageMod>();

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);

        float totalDamage = caster.myStats.Power * powerMod;

        damageMods.ForEach((damageMod) => {
            totalDamage = damageMod.Apply(totalDamage, caster, TargetUnit);
        });

        caster.DealDamageTo(TargetUnit, Mathf.RoundToInt(totalDamage));
    }
}