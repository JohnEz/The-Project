using UnityEngine;

public enum ForcedMovementType {
    PUSH,
    PULL
}

[CreateAssetMenu(fileName = "New Attack Action", menuName = "Card/Attack/Force Movement")]
public class ForceMovementEffect : AttackEffect {
    public ForcedMovementType movementType = ForcedMovementType.PUSH;

    public int distance = 1;

    public override void AbilityEffect(UnitController caster, UnitController target) {
        if (movementType == ForcedMovementType.PUSH) {
            target.Push(caster.myNode, distance);
        } else {
            target.Pull(caster.myNode, distance);
        }
    }
}