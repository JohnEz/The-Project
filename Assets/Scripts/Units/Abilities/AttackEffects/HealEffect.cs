using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New damage effect", menuName = "Ability/Attack/Heal")]
public class HealEffect : AttackEffect {
    public int woundsHealed = 1;

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);

        if (!targetNode.MyUnit) {
            return;
        }

        UnitController targetUnit = targetNode.MyUnit;
        for (int i = 0; i < woundsHealed; i++) {
            HitLocation hitLocation = targetUnit.myStats.GetRecentWoundedHitLocation();

            if (hitLocation) {
                targetUnit.HitLocationHealed(hitLocation, caster);
            }
        }
    }
}