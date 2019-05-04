﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Damage/Damage")]
public class DamageEffect : AttackEffect {
    public int damage = 1;

    public List<DamageMod> damageMods = new List<DamageMod>();

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);

        float totalDamage = damage;

        damageMods.ForEach((damageMod) => {
            totalDamage = damageMod.Apply(totalDamage, caster, TargetUnit);
        });

        caster.DealDamageTo(TargetUnit, Mathf.RoundToInt(totalDamage));
    }
}