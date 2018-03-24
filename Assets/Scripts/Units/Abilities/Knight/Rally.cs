using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rally : BaseAbility {

	int buffDuration = 3;

	public Rally(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		targets = TargetType.ALLY;
		areaOfEffect = AreaOfEffect.AURA;
		tileTarget = TileTarget.TILE;
		maxCooldown = 3;
		aoeRange = 4;
		icon = "abilityRallyController";
		CanTargetSelf = true;
		Name = "Rally";
	}

	public override void UseAbility (Node target)
	{
		if (CanHitUnit(target)) {
			caster.AddAbilityTarget (target.myUnit, () => {
				target.myUnit.ApplyBuff (new Empower (buffDuration));
				target.myUnit.ApplyBuff (new Momentum (buffDuration));
			});
		}
	}

	public override void UseAbility(List<Node> targets, Node targetedNode) {
		base.UseAbility (targets, targetedNode);
		targets.ForEach (target => UseAbility (target));
	}

	public override string GetDescription() {
		return base.GetDescription() + "Boosts allies moral increasing their movement and power for " + buffDuration + " turns.";
	}
}
