using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Damage Per Stack")]
public class DamagePerStackEffect : AttackEffect {
    public int baseDamage = 1;
    public int damageMod = 1;
    public string buffName = "Burn";

    public bool consumeBuff = false;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        Buff targetBuff = target.myStats.FindBuff(buffName);
        int numberOfStacks = targetBuff != null ? targetBuff.stacks : 0;
        int damage = baseDamage + (numberOfStacks * damageMod);

        caster.DealDamageTo(target, damage);

        if (consumeBuff) {
            target.myStats.RemoveBuff(targetBuff);
        }
    }

    public override string ToDescription() {
        string consumeString = consumeBuff ? " Consumes " + buffName + "." : "";
        return string.Format("Deal {0} damage per stack of {1}.{2}", damageMod, buffName, consumeString);
    }
}