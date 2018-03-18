using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JadeSlam : BaseAbility {

	float damageMod = 1;

	public JadeSlam (List<EventAction> _eventActions) : base (_eventActions) {
		Name = "Jade Slam";
		Description = "Range: " + range + "\nSlam an enemy with all your might dealing " + damageMod + "x power damage.";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

}
