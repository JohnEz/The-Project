using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SacredGround : BaseAbility {

	public SacredGround(List<EventAction> _eventActions) : base (_eventActions) {
		targets = TargetType.ALLY;
		areaOfEffect = AreaOfEffect.AURA;
		tileTarget = TileTarget.TILE;
		aoeRange = 3;
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		if (CanHitUnit(caster, target)) {
			caster.AddAbilityTarget (target.myUnit, () => {
				target.myUnit.Dispell(true);
			});
		}
	}

	public override void UseAbility(UnitController caster, List<Node> targets, Node targetedNode) {
		base.UseAbility (caster, targets, targetedNode);
		targets.ForEach (target => UseAbility (caster, target));
	}
}
