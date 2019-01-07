using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Damage")]
public class DamageEffect : AttackEffect {
    public int damage = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        caster.DealDamageTo(target, damage);
    }
}