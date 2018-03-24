using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcaneBolt : BaseAbility {

	float damageMod = 0.75f;

	public ArcaneBolt(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		range = 6;
		icon = "abilityArcaneBoltController";
		Name = "Arcane Bolt";
	}

	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		AddAbilityTarget (target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Sends a bolt of arcane energy at an enemy dealing " + (int)(damageMod * caster.myStats.Power) + " damage.";
	}

}
