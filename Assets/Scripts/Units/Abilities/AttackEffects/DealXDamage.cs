using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Deal X Damage")]
public class DealXDamage : AttackEffect {

    public int damage = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        caster.DealDamageTo(target, damage);
    }
}
