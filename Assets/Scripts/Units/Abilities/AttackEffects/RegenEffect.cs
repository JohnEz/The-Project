using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Regen")]
public class Regen : AttackEffect {
    public int turns = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        target.ApplyBuff(new Vitalise(turns));
    }
}