using UnityEngine;

public enum ForcedMovementType {
    PUSH,
    PULL
}

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Force Movement")]
public class ForceMovementEffect : AttackEffect {
    public ForcedMovementType movementType = ForcedMovementType.PUSH;

    public int distance = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        base.AbilityEffect(caster, target);
        if (movementType == ForcedMovementType.PUSH) {
            TargetUnit.Push(caster.myNode, distance);
        } else {
            TargetUnit.Pull(caster.myNode, distance);
        }
    }
}