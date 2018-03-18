using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cauterize : BaseAbility {

	float healingMod = 2f;
	int duration = 3;

	public Cauterize(List<EventAction> _eventActions) : base (_eventActions) {
		range = 4;
		maxCooldown = 1;
		targets = TargetType.ALLY;
		icon = "abilityEngulfController";
		CanTargetSelf = true;
		Name = "Cauterize";
		Description = "Range: " + range + "\nClose wounds of an ally with fire. Healing them for " + healingMod + "x power but applying a " + duration + " turn burn.";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {
			caster.GiveHealingTo(target.myUnit, healingMod);
			target.myUnit.ApplyBuff(new Burn(duration, caster.myStats.Power));
		});
	}

}
