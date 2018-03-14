using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rally : BaseAbility {

	int buffDuration = 3;

	public Rally(List<EventAction> _eventActions) : base (_eventActions) {
		targets = TargetType.ALLY;
		areaOfEffect = AreaOfEffect.AURA;
		tileTarget = TileTarget.TILE;
		maxCooldown = 3;
		aoeRange = 4;
		icon = "abilityRallyController";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		if (CanHitUnit(caster, target)) {
			caster.AddAbilityTarget (target.myUnit, () => {
				target.myUnit.ApplyBuff (new Empower (buffDuration));
				target.myUnit.ApplyBuff (new Momentum (buffDuration));
			});
		}
	}

	public override void UseAbility(UnitController caster, List<Node> targets, Node targetedNode) {
		base.UseAbility (caster, targets, targetedNode);
		targets.ForEach (target => UseAbility (caster, target));
	}
}
