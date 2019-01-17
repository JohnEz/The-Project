using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Increase Power")]
public class IncreasePower : AttackEffect {
    public int turns = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        target.ApplyBuff(new Empower(turns));
    }
}