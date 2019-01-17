using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Increase Speed")]
public class IncreaseSpeed : AttackEffect {
    public int turns = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        target.ApplyBuff(new Momentum(turns));
    }
}