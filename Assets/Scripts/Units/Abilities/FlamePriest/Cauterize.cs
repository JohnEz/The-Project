using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cauterize : BaseAbility {

	float healingMod = 2f;

	public Cauterize(List<EventAction> _eventActions) : base (_eventActions) {
		range = 4;
		maxCooldown = 1;
		targets = TargetType.ALLY;
		icon = "abilityEngulfController";
		CanTargetSelf = true;
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {
			caster.GiveHealingTo(target.myUnit, healingMod);
			target.myUnit.ApplyBuff(new Burn(3, caster.myStats.Power));
		});
	}

}
