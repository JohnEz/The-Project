using UnityEngine;

[CreateAssetMenu(fileName = "New Slow Action", menuName = "Card/Attack/Slow")]
public class SlowEffect : AttackEffect {
    public int duration = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        target.ApplyBuff(new Slow(duration));
    }
}