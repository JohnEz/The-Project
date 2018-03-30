using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlamingArrow : BaseAbility {

	float damageMod = 1f;
	int duration = 3;

	public FlamingArrow(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		range = 6;
		icon = "abilityArcaneBoltController";
		Name = "Flaming Arrow";
	}

	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		AddAbilityTarget (target.myUnit, () => {
			bool targetStillAlive = caster.DealDamageTo(target.myUnit, damageMod);
			if (targetStillAlive) {
				target.myUnit.ApplyBuff(new Burn(duration, caster.myStats.Power));
			}
		});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Shoots a flaming arrow that burns an enemy for " + (int)(damageMod * caster.myStats.Power) + " and applies a " + duration + " turn burn.";
	}

}
