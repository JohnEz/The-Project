using UnityEngine;

public enum RelativePosition {
    REAR,
    FRONT,
    FLANK,
    NOT_IN_FRONT
}

[CreateAssetMenu(fileName = "Per Shield Mod", menuName = "Ability/Attack/Damage/Per Position")]
public class DamagePerPosition : DamageMod {
    public float damageMod = 1f;
    public RelativePosition position = RelativePosition.REAR;

    public override float Apply(float damage, UnitController caster, UnitController target) {
        Vector2 direction = caster.GetDirectionToTile(target.myNode);

        bool isInPosition = false;

        switch (position) {
            case RelativePosition.REAR:
                isInPosition = direction.Equals(target.facingDirection);
                break;

            case RelativePosition.FRONT:
                isInPosition = direction.Equals(target.facingDirection * -1);
                break;

            case RelativePosition.FLANK:
                isInPosition = Mathf.Abs(direction.x).Equals(Mathf.Abs(target.facingDirection.y));
                break;

            case RelativePosition.NOT_IN_FRONT:
                isInPosition = !direction.Equals(target.facingDirection * -1);
                break;
        }

        float finalDamageMod = isInPosition ? damageMod : 1f;
        return damage * finalDamageMod;
    }
}