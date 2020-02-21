using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Damage/Damage")]
public class DamageEffect : AttackEffect {
    public DamageType damageType = DamageType.BLUDGEONING;
    public int damageMin = 1;
    public int damageMax = 1;

    public List<DamageMod> damageMods = new List<DamageMod>();

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);

        int damage = Random.Range(damageMin, damageMax);

        float totalDamage = damage + GetStatMod(caster);

        damageMods.ForEach((damageMod) => {
            totalDamage = damageMod.Apply(totalDamage, caster, targetNode.MyUnit);
        });

        caster.DealDamageTo(targetNode.MyUnit, Mathf.RoundToInt(totalDamage));
    }

    public int GetStatMod(UnitController caster) {
        int strengthDamage = caster.myStats.Strength;
        int agilityDamage = caster.myStats.Agility;

        switch (damageType) {
            case DamageType.BLUDGEONING:
                return strengthDamage;

            case DamageType.PIERCING:
                return agilityDamage;

            case DamageType.SLASHING:
                return strengthDamage > agilityDamage ? strengthDamage : agilityDamage;

            default: return 0;
        }
    }
}