using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Engulf : BaseAbility {

	float damageMod = 0.75f;
	int duration = 3;

	public Engulf(List<EventAction> _eventActions, UnitStats casterStats) : base (_eventActions, casterStats) {
		range = 5;
		icon = "abilityEngulfController";
		Name = "Engulf";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {
			bool targetStillAlive = caster.DealDamageTo(target.myUnit, damageMod);
			if (targetStillAlive) {
				target.myUnit.ApplyBuff(new Burn(duration, caster.myStats.Power));
			}
		});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Burns an enemy for " + (int)(damageMod * casterStats.Power) + " and applies a " + duration + " turn burn.";
	}

}
