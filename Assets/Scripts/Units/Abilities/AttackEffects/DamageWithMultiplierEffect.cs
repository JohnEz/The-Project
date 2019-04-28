using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Damage with Multiplier")]
public class DamageWithMultiplierEffect : DamagePerStackEffect {

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        Buff targetBuff = target.myStats.FindBuff(buffName);
        int damage = baseDamage * (targetBuff != null ? damageMod : 1);

        caster.DealDamageTo(TargetUnit, damage);

        if (consumeBuff) {
            TargetUnit.myStats.RemoveBuff(targetBuff);
        }
    }

    public override string ToDescription() {
        string consumeString = consumeBuff ? " Consumes " + buffName + "." : "";
        return string.Format("Deal {0} damage. If the target has {1}, mutliply by {2}.{3}", baseDamage, buffName, damageMod, consumeString);
    }
}