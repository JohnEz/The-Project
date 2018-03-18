using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcaneBolt : BaseAbility {

	float damageMod = 0.75f;

	public ArcaneBolt(List<EventAction> _eventActions) : base (_eventActions) {
		range = 6;
		icon = "abilityArcaneBoltController";
		Name = "Arcane Bolt";
		Description = "Range: " + range + "\nSends a bolt of arcane energy at an enemy dealing " + damageMod + "x power damage.";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

}
