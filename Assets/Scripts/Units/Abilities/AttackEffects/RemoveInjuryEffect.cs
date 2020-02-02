using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New damage effect", menuName = "Ability/Attack/Remove Injury")]
public class RemoveInjuryEffect : AttackEffect {

    public override void AbilityEffect(UnitController caster, Node targetNode) {
        base.AbilityEffect(caster, targetNode);
    }
}