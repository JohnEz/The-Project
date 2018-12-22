using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Damage with Multiplier")]
public class DamageWithMultiplier : DamagePerStackEffect {

    public int baseDamage = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        Buff targetBuff = target.myStats.FindBuff(buffName);
        int damage = baseDamage * (targetBuff != null ? damageMod : 1);

        caster.DealDamageTo(target, damage);

        if (consumeBuff) {
            target.myStats.RemoveBuff(targetBuff);
        }
    }

    public override string ToDescription() {
        string consumeString = consumeBuff ? " Consumes " + buffName + "." : "";
        return string.Format("Deal {0} damage. If the target has {1}, mutliply by {2}.{3}", baseDamage, buffName, damageMod, consumeString);
    }
}
