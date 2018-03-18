using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcaneBolt : BaseAbility {

	float damageMod = 0.75f;

	public ArcaneBolt(List<EventAction> _eventActions, UnitStats casterStats) : base (_eventActions, casterStats) {
		range = 6;
		icon = "abilityArcaneBoltController";
		Name = "Arcane Bolt";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

	public override string GetDescription() {
		return "Range: " + range + "\nSends a bolt of arcane energy at an enemy dealing " + (int)(damageMod * casterStats.Power) + " damage.";
	}

}
