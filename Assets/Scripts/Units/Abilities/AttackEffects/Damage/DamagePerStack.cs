using UnityEngine;

[CreateAssetMenu(fileName = "Per Stack Mod", menuName = "Ability/Attack/Damage/Per Stack")]
public class DamagePerStack : DamageMod {
    public string buffName = "Burn";

    public bool consumeBuff = false;

    public override int Apply(int damage, UnitController caster, UnitController target) {
        Buff targetBuff = target.myStats.FindBuff(buffName);
        int numberOfStacks = targetBuff != null ? targetBuff.stacks : 0;

        if (consumeBuff) {
            target.myStats.RemoveBuff(targetBuff);
        }

        return damage * numberOfStacks;
    }
}