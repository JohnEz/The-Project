using UnityEngine;

[CreateAssetMenu(fileName = "Per Stack Mod", menuName = "Ability/Attack/Damage/Per Stack")]
public class DamagePerStack : DamageMod {
    public string buffName = "Burn";

    public bool consumeBuff = false;

    public override float Apply(float damage, UnitController caster, UnitController target) {
        Buff targetBuff = target.myStats.buffs.FindBuff(buffName);
        if (targetBuff == null) {
            return 0;
        }

        int numberOfStacks = targetBuff.stacks;

        if (consumeBuff) {
            target.myStats.RemoveBuff(targetBuff);
        }

        return damage * numberOfStacks;
    }
}