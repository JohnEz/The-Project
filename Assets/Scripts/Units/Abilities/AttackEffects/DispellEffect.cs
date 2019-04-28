using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Dispell")]
public class DispellEffect : AttackEffect {
    public int dispellCount = 1;
    public bool debuff = true;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);

        if (dispellCount == -1) {
            target.DispellAll(debuff);
        } else {
            for (int i = 0; i < dispellCount; i++) {
                target.Dispell(debuff);
            }
        }
    }
}