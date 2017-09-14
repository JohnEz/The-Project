using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cauterize : BaseAbility {

	int baseHealing = 8;

	public Cauterize(List<EventAction> _eventActions) : base (_eventActions) {
		range = 4;
		targets = TargetType.ALLY;
		icon = "abilityEngulfController";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.GiveHealingTo(target.myUnit, baseHealing);});
	}

}
