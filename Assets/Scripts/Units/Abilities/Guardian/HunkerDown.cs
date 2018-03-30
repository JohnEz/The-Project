using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HunkerDown : BaseAbility {

	int buffDuration = 3;

	public HunkerDown(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		targets = TargetType.ALLY;
		areaOfEffect = AreaOfEffect.AURA;
		tileTarget = TileTarget.UNIT;
		maxCooldown = 3;
		aoeRange = 0;
		icon = "abilityRallyController";
		CanTargetSelf = true;
		Name = "Hunker Down";
	}

	public override void UseAbility (Node target)
	{
		if (CanHitUnit(target)) {
			caster.AddAbilityTarget (target.myUnit, () => {
				target.myUnit.ApplyBuff (new Snare (buffDuration));
				target.myUnit.ApplyBuff (new Armour (buffDuration));
				target.myUnit.ApplyBuff (new Armour (buffDuration));
				target.myUnit.ApplyBuff (new Armour (buffDuration));
				target.myUnit.ApplyBuff (new Armour (buffDuration));
				target.myUnit.ApplyBuff (new Armour (buffDuration));
			});
		}
	}

	public override void UseAbility(List<Node> targets, Node targetedNode) {
		base.UseAbility (targets, targetedNode);
		targets.ForEach (target => UseAbility (target));
	}

	public override string GetDescription() {
		return base.GetDescription() + "Holds his current position, stoping movement but giving 5 stacks of armour for " + buffDuration + " turns.";
	}
}