using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Deal X Damage")]
public class DealXDamage : AttackAction {

    public int damage = 1;

    public override void UseAbility(Node target) {
        base.UseAbility(target);
        AddAbilityTarget(target.myUnit, () => {
            caster.DealDamageTo(target.myUnit, damage);
        });
    }
}
