using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HolyStrike : BaseAbility {

	float damageMod = 1;
	float healingMod = 0.25f;

	public HolyStrike(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		icon = "abilityHolyStrikeController";
		Name = "Holy Strike";
	}

	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		AddAbilityTarget (target.myUnit, () => {
			caster.DealDamageTo(target.myUnit, damageMod);
			caster.GiveHealingTo(caster, healingMod);
		});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Attack with holy might dealing " + (int)(damageMod * caster.myStats.Power) + " damage and healing the knight for " + (int)(healingMod * caster.myStats.Power) + ".";
	}

}
