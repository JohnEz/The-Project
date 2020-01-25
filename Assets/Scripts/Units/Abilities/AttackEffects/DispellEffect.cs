using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Dispell")]
public class DispellEffect : AttackEffect {
    public int dispellCount = 1;
    public bool debuff = true;

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);

        if (dispellCount == -1) {
            targetNode.MyUnit.DispellAll(debuff);
        } else {
            for (int i = 0; i < dispellCount; i++) {
                targetNode.MyUnit.Dispell(debuff);
            }
        }
    }
}