using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cleave : BaseAbility {

	float damageMod = 0.5f;
	int duration = 5;

	public Cleave(List<EventAction> _eventActions, UnitStats casterStats) : base (_eventActions, casterStats) {
		range = 1;
		areaOfEffect = AreaOfEffect.CLEAVE;
		aoeRange = 1;
		maxCooldown = 2;
		tileTarget = TileTarget.TILE;
		icon = "abilityFrostNovaController";
		Name = "Cleave";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		if (CanHitUnit(caster, target)) {
			AddAbilityTarget (caster, target.myUnit, () => {
				bool targetStillAlive = caster.DealDamageTo(target.myUnit, damageMod);
				if (targetStillAlive) {
					target.myUnit.ApplyBuff (new Burn (duration, caster.myStats.Power));
				}
			});
		}
	}

	public override void UseAbility(UnitController caster, List<Node> targets, Node targetedNode) {
		base.UseAbility (caster, targets, targetedNode);
		targets.ForEach (target => UseAbility (caster, target));
	}

	public override string GetDescription() {
		return "Range: " + range + "\nFreezes ememies in an area, dealing " + (int)(damageMod * casterStats.Power) + " damage and snaring them for " + duration + " turns.";
	}

}
