using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Increase Armour")]
public class IncreaseArmour : AttackEffect {
    public int turns = 1;
    public int armourIncrease = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        target.ApplyBuff(new Armour(turns, 1));
    }
}