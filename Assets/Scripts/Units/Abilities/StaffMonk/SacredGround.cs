using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SacredGround : BaseAbility {

	float healingMod = 0.5f;

	public SacredGround(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		targets = TargetType.ALLY;
		areaOfEffect = AreaOfEffect.AURA;
		tileTarget = TileTarget.TILE;
		CanTargetSelf = true;
		maxCooldown = 2;
		aoeRange = 3;
		Name = "Sacred Ground";
	}

	public override void UseAbility (Node target)
	{
		if (CanHitUnit(target)) {
			AddAbilityTarget (target.myUnit, () => {target.myUnit.Dispell(true);});
			AddAbilityTarget (target.myUnit, () => {caster.GiveHealingTo(target.myUnit, healingMod);});
		}
	}

	public override void UseAbility(List<Node> targets, Node targetedNode) {
		base.UseAbility (targets, targetedNode);
		targets.ForEach (target => UseAbility (target));
	}

	public override string GetDescription() {
		return base.GetDescription() + "Creates an area of sacred ground healing allies for " + (int)(healingMod * caster.myStats.Power) + " and removing 1 negative effect from them.";
	}
}
