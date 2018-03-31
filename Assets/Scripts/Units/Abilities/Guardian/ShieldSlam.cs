using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShieldSlam : BaseAbility {

	int duration = 2;
	float damageMod = 0.75f;

	public ShieldSlam(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		icon = "abilityHolyStrikeController";
		Name = "Hunker Down";
	}

	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		AddAbilityTarget (target.myUnit, () => {
			bool targetStillAlive = caster.DealDamageTo(target.myUnit, damageMod);
			if (targetStillAlive) {
				target.myUnit.ApplyBuff(new Slow(duration));
			}
		});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Slams the target with his shield dealing " + (int)(damageMod * caster.myStats.Power) + " damage applies slow for " + duration + " turns.";
	}
}