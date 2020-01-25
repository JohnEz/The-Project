using UnityEngine;

public enum ForcedMovementType {
    PUSH,
    PULL
}

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Force Movement")]
public class ForceMovementEffect : AttackEffect {
    public ForcedMovementType movementType = ForcedMovementType.PUSH;

    public int distance = 1;

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);
        if (movementType == ForcedMovementType.PUSH) {
            targetNode.MyUnit.Push(caster.myTile, distance);
        } else {
            targetNode.MyUnit.Pull(caster.myTile, distance);
        }
    }
}