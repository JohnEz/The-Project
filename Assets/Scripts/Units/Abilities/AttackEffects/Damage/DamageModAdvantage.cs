using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Damage/Advantage Mod")]
public class DamageModAdvantage : DamageMod {
    public float damageMod = 1f;

    public override float Apply(float damage, UnitController caster, UnitController target) {
        //TODO update to allow none and dissadvantage

        return AttackAction.AttackerHasAdvantage(caster, target) ? damage * damageMod : damage;
    }
}