using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JadeSlam : BaseAbility {

	float damageMod = 1;

	public JadeSlam (List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		Name = "Jade Slam";
	}

	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		AddAbilityTarget (target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Slam an enemy with all your might dealing " + (int)(damageMod * caster.myStats.Power) + " damage.";
	}

}
