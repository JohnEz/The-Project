using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SacredGround : BaseAbility {

	float healingMod = 0.5f;

	public SacredGround(List<EventAction> _eventActions) : base (_eventActions) {
		targets = TargetType.ALLY;
		areaOfEffect = AreaOfEffect.AURA;
		tileTarget = TileTarget.TILE;
		CanTargetSelf = true;
		maxCooldown = 2;
		aoeRange = 3;
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		if (CanHitUnit(caster, target)) {
			AddAbilityTarget (caster, target.myUnit, () => {target.myUnit.Dispell(true);});
			AddAbilityTarget (caster, target.myUnit, () => {caster.GiveHealingTo(target.myUnit, healingMod);});
		}
	}

	public override void UseAbility(UnitController caster, List<Node> targets, Node targetedNode) {
		base.UseAbility (caster, targets, targetedNode);
		targets.ForEach (target => UseAbility (caster, target));
	}
}
