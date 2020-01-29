using UnityEngine;

public enum ForcedMovementType {
    FEAR,
    PUSH,
    PULL
}

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Ability/Attack/Force Movement")]
public class ForceMovementEffect : AttackEffect {
    public ForcedMovementType movementType = ForcedMovementType.PUSH;

    public int distance = 1;

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);
        if (targetNode.MyUnit == null) {
            Debug.LogError("Tried to use force move on empty tile");
            Debug.LogError(targetNode);
            return;
        }

        switch (movementType) {
            case ForcedMovementType.PUSH:
                targetNode.MyUnit.Push(caster.myTile, distance);
                break;

            case ForcedMovementType.PULL:
                targetNode.MyUnit.Pull(caster.myTile, distance);
                break;

            case ForcedMovementType.FEAR:
                targetNode.MyUnit.Fear(caster.myTile, distance);
                break;
        }
    }
}