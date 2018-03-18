using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HolyStrike : BaseAbility {

	float damageMod = 1;
	float healingMod = 0.25f;

	public HolyStrike(List<EventAction> _eventActions, UnitStats casterStats) : base (_eventActions, casterStats) {
		icon = "abilityHolyStrikeController";
		Name = "Holy Strike";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {
			caster.DealDamageTo(target.myUnit, damageMod);
			caster.GiveHealingTo(caster, healingMod);
		});
	}

	public override string GetDescription() {
		return "Range: " + range + "\nAttack with holy might dealing " + (int)(damageMod * casterStats.Power) + " damage and healing the knight for " + (int)(healingMod * casterStats.Power) + ".";
	}

}
