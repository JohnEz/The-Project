using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcaneBolt : BaseAbility {

	float damageMod = 0.75f;

	public ArcaneBolt(List<EventAction> _eventActions) : base (_eventActions) {
		range = 6;
		icon = "abilityArcaneBoltController";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

}
