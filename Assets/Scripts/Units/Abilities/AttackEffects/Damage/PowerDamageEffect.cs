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

    public override int GetDamage(UnitController caster) {
        float totalDamage = caster.myStats.Power * powerMod;
        DamagePerShield perShieldEffect = (DamagePerShield)damageMods.Find(effect => effect.GetType() == typeof(DamagePerShield));

        if (perShieldEffect != null) {
            totalDamage = perShieldEffect.Apply(totalDamage, caster, null);
        }

        return Mathf.RoundToInt(totalDamage);
    }
}