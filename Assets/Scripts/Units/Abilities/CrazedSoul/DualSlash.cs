using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DualSlash : BaseAbility {

	float damageMod = 1.25f;

	public DualSlash(List<EventAction> _eventActions, UnitController caster) : base (_eventActions, caster) {
		Name = "Dual Slash";
	}

	public override void UseAbility (Node target)
	{
		base.UseAbility (target);
		AddAbilityTarget (target.myUnit, () => {caster.DealDamageTo(target.myUnit, damageMod);});
	}

	public override string GetDescription() {
		return base.GetDescription() + "Attack using both blades dealing " + (int)(damageMod * caster.myStats.Power) + " damage.";
	}
}
