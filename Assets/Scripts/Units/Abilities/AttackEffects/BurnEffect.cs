using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Burn")]
public class BurnEffect : AttackEffect {

    public int damage = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        target.ApplyBuff(new Burn(3, damage));
    }
}
