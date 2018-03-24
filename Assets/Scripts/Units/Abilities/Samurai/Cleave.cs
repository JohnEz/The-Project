using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cleave : BaseAbility {

	float damageMod = 0.5f;
	int duration = 5;

	public Cleave(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		range = 1;
		areaOfEffect = AreaOfEffect.CLEAVE;
		aoeRange = 1;
		maxCooldown = 2;
		tileTarget = TileTarget.TILE;
		Name = "Cleave";
	}

	public override void UseAbility (Node target)
	{
		if (CanHitUnit(target)) {
			AddAbilityTarget (target.myUnit, () => {
				bool targetStillAlive = caster.DealDamageTo(target.myUnit, damageMod);
				if (targetStillAlive) {
					target.myUnit.ApplyBuff (new Bleed (duration, caster.myStats.Power));
				}
			});
		}
	}

	public override void UseAbility(List<Node> targets, Node targetedNode) {
		base.UseAbility (targets, targetedNode);
		targets.ForEach (target => UseAbility (target));
	}

	public override string GetDescription() {
		return base.GetDescription() + "Cleaves up to 3 targets infront of the samurai for " + (int)(damageMod * caster.myStats.Power) + " damage and applies a " + duration + " turn burn to them.";
	}

}
