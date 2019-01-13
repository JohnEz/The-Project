using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Heal")]
public class HealEffect : AttackEffect {
    public int healing = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        caster.GiveHealingTo(target, healing);
    }
}