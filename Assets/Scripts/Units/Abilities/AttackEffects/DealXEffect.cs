using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/X Damage")]
public class DealXEffect : AttackEffect {

    public int baseDamage = 0;
    public int damagePerStack = 1;

    public string buffName = "Burn";

    public bool consumeBuff = false;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        Buff targetBuff = target.myStats.FindBuff(buffName);
        int numberOfStacks = targetBuff.stacks;
        int damage = baseDamage + (numberOfStacks * damagePerStack);

        caster.DealDamageTo(target, damage);

        if (consumeBuff) {
            target.myStats.RemoveBuff(targetBuff);
        }
    }
}
