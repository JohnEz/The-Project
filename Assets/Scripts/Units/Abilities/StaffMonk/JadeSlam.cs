using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JadeSlam : BaseAbility {

	float damageMod = 1;

	public JadeSlam (List<EventAction> _eventActions, UnitStats casterStats) : base (_eventActions, casterStats) {
		Name = "Jade Slam";
	}

	public override void UseAbility (UnitController caster, Node target)
	{
		base.UseAbility (caster, target);
		AddAbilityTarget (caster, target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

	public override string GetDescription() {
		return "Range: " + range + "\nSlam an enemy with all your might dealing " + (int)(damageMod * casterStats.Power) + " damage.";
	}

}
