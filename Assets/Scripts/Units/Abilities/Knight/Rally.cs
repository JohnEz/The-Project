using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rally : BaseAbility {

	public Rally(List<EventAction> _eventActions) : base (_eventActions) {
		targets = TargetType.ALLY;
		areaOfEffect = AreaOfEffect.AURA;
		tileTarget = TileTarget.TILE;
		aoeRange = 4;
		icon = "abilityRallyController";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		if (CanHitUnit(caster, target)) {
			caster.AddAbilityTarget (target.myUnit, () => {
				target.myUnit.ApplyBuff (new Empower (3));
				target.myUnit.ApplyBuff (new Momentum (3));
			});
		}
	}

	public override void UseAbility(UnitController caster, List<Node> targets, Node targetedNode) {
		base.UseAbility (caster, targets, targetedNode);
		targets.ForEach (target => UseAbility (caster, target));
	}
}
