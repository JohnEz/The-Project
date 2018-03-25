﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cauterize : BaseAbility {

	float healingMod = 2f;
	int duration = 3;

	public Cauterize(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		range = 4;
		maxCooldown = 1;
		targets = TargetType.ALLY;
		icon = "abilityEngulfController";
		CanTargetSelf = true;
		Name = "Cauterize";
	}

	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		AddAbilityTarget (target.myUnit, () => {
			caster.GiveHealingTo(target.myUnit, healingMod);
			target.myUnit.ApplyBuff(new Burn(duration, caster.myStats.Power));
		});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Close wounds of an ally with fire. Healing them for " + (int)(healingMod * caster.myStats.Power)  + " but applys a " + duration + " turn burn.";
	}

}